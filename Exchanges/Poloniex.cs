namespace Exchanges
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Core;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using ExchangesCore.ExchangeAPICallers;
    using Logging;
    using CurrencyCore.Coin;
    using CurrencyCore.Address;
    using ExchangesCore;
    using Newtonsoft.Json.Linq;
    using System.Threading;

    public class Poloniex : ExchangeBase
    {
        private static readonly ConcurrentQueue<Action> RequestQueue = new ConcurrentQueue<Action>();

        private static Task CurrentRequest;
        static Poloniex() 
        {
            System.Timers.Timer timer = new System.Timers.Timer()
            {
                AutoReset = true,
                //Rate has to be less than 6 per second
                Interval = (1000.0 * 1.0) / 5.0
            };
            timer.Elapsed += SendMessage;
            timer.Start();
        }

        private static void SendMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentRequest == null || CurrentRequest.IsCompleted)
            {
                lock (RequestQueue)
                {
                    if (RequestQueue.TryDequeue(out var request))
                    {
                        CurrentRequest = Task.Run(request);
                    }
                }
            }
        }

        private const string PrivateUrl = "https://poloniex.com/tradingApi";
        private const string PublicUrl = "https://poloniex.com/public";
        private readonly RestCall privateCaller;
        private readonly RestCall publicCaller;

        public Poloniex(ILogger logger, KeyPair<string, string> apiKeys = null) : base(nameof(Poloniex))
        {
            if (apiKeys != null)
            {
                this.privateCaller = new HmacSha512SignedPostCall(new Uri(PrivateUrl), logger, apiKeys, RequestQueue);
            }
            this.publicCaller = new GetCall(new Uri(PublicUrl), logger, RequestQueue);
        }

        public override async ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees()
        {
            var resultContainer = await this.SendCommand<JContainer>("returnFeeInfo");
            if (resultContainer != null)
            {
                var maker = new Fee(Decimal.Parse(resultContainer["makerfee"].Value<string>()), CoinAmount.Zero);
                var taker = new Fee(Decimal.Parse(resultContainer["takerfee"].Value<string>()), CoinAmount.Zero);
                return (maker, taker);
            }
            return (null, null);
        }

        public override async ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount, bool postOnly = false)
        {
            
            var command = trade.Type == TradeType.Buy ? "buy" : "sell";
            var paramDictionary = new Dictionary<string, string>
            {
               {"currencyPair", $"{this.GetCurrencyPair(trade.StockCoin, trade.CurrencyCoin)}" },
               {"rate", price.ToString()},
               {"amount", amount.ToString()}
            };
            if (postOnly)
            {
                paramDictionary.Add("postOnly", "1");
            }

            var resultContainer = await this.SendCommand<JContainer>(command, paramDictionary);
            return true;

        }

       
        public override async ValueTask<Price?> GetEstimatedBuyExchangeRate(CoinType stockCoin, CoinType currencyCoin)
        {
            var history = await this.GetTradeHistory(stockCoin, currencyCoin);

            var (asks, bids) = await this.GetOrderBook(stockCoin, currencyCoin);
            var range = this.GetEstimatedExchangeRate(history, asks);
            return range.HasValue ? (Price?)(range.Value.Max) : null ;
        }
        public override async ValueTask<Price?> GetEstimatedSellExchangeRate(CoinType stockCoin, CoinType currencyCoin)
        {
            var history = await this.GetTradeHistory(stockCoin, currencyCoin);

            var (asks, bids) = await this.GetOrderBook(stockCoin, currencyCoin);
            var range = this.GetEstimatedExchangeRate(history, bids);
            return range.HasValue ? (Price?)(range.Value.Min) : null;
        }           

        public override async ValueTask<CoinAmount?> GetBalance(CoinType coinType)
        {
            var balances = await this.GetBalances();
            
            if (balances != null && balances.TryGetValue(coinType, out var result))
            {
                return result;
            }

            return null;
        }

        public override async ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances()
        {
            return await this.GetValueFromCache<Task<Dictionary<CoinType, CoinAmount>>>("Balances", async () =>
            {
                var jsonResult = await this.SendCommand<Dictionary<string, decimal>>("returnBalances");
                if (jsonResult != null)
                {
                    var result = new Dictionary<CoinType, CoinAmount>();
                    foreach (var key in jsonResult.Keys)
                    {
                        var coinType = CoinInfo.GetCoinType(key);
                        if (!result.ContainsKey(coinType))
                        {
                            result.Add(coinType, (CoinAmount)jsonResult[key]);
                        }
                    }
                    return result;
                }
                return null;
            },
            secondsTilExpiration:60);
        }
        
        public override async ValueTask<string> Withdraw(PublicAddress address, CoinAmount amount)
        {
            var paramDictionary = new Dictionary<string, string>
            {
               {"currency", CoinInfo.GetDefaultAbbreviation(address.AddressType)},
               {"amount", amount.ToString()},
               {"address", address.ToString()}
            };

            return await this.SendCommand("withdraw", paramDictionary);
        }

        public override async ValueTask<List<CompletedTrade>> GetTradeHistory(CoinType stockType, CoinType currencyType, int depth = 50)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "TradeHistory:" + currencyPair;

            return await this.GetValueFromCache<Task<List<CompletedTrade>>>(key, async () =>
            {
                var paramDictionary = new Dictionary<string, string>
                {
                    {"currencyPair",  currencyPair},
                    {"depth", depth.ToString()}
                };

                var response = await this.SendCommand<JContainer>("returnTradeHistory", paramDictionary);
                List<CompletedTrade> result = null;
                if (response != null)
                {
                    result = new List<CompletedTrade>();
                    foreach (var trade in response)
                    {
                        var type = trade["type"].Value<string>().Equals("sell") ? TradeType.Sell : TradeType.Buy;
                        var date = trade["date"].Value<DateTime>();
                        var rate = (Price)trade["rate"].Value<decimal>();
                        var amount = (CoinAmount)trade["amount"].Value<decimal>();
                        result.Add(new CompletedTrade(type, stockType, currencyType, rate, amount, date));
                    }
                }
                return result;
            },
            secondsTilExpiration:20);
        }
        public override async ValueTask<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>> GetOrderBooks(int depth = 50)
        {
            return await this.GetValueFromCache("orderbook", async () =>
            {
                var result = new Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>(StringComparer.InvariantCultureIgnoreCase);
                var paramDictionary = new Dictionary<string, string>
             {
                {"currencyPair", "all" },
                {"depth", depth.ToString()}
             };
                var response = await this.SendCommand<Dictionary<string, JContainer>>("returnOrderBook", paramDictionary);
                if (response != null)
                {
                    foreach (var market in response)
                    {
                        var pair = market.Key;
                        result.Add(pair, ParseOrderBook(market.Value));
                    }
                    return result;
                }
                return null;
            }
            );
        }

        public override async ValueTask<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)> GetOrderBook(CoinType stockType, CoinType currencyType, int depth = 50)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "OrderBook:" + currencyPair;
            return await this.GetValueFromCache(key, async () =>
            {
                var fullOrderDictionary = this.GetValueFromCache<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>>("orderbook", () => null);
                if(fullOrderDictionary != null && fullOrderDictionary.TryGetValue(currencyPair, out var cachedResult))
                {
                    return cachedResult;
                }

                var paramDictionary = new Dictionary<string, string>
                {
                    {"currencyPair", currencyPair },
                    {"depth", depth.ToString()}
                };

                (List<(Price, CoinAmount)>, List<(Price, CoinAmount)>) book = ((List<(Price, CoinAmount)>)null, (List<(Price, CoinAmount)>)null);

                var response = await this.SendCommand<JContainer>("returnOrderBook", paramDictionary);

                if (response != null)
                {
                    book = ParseOrderBook(response);
                }
                return book;
            });
        }

        public override async ValueTask<List<LoanOffer>> GetLoanOrderBook(CoinType currencyType, int depth = 50)
        {
            var paramDictionary = new Dictionary<string, string>
            {
                {"currency", $"{CoinInfo.GetDefaultAbbreviation(currencyType)}" },
                {"limit", depth.ToString()}
            };

            var resultContainer = await this.SendCommand<JContainer>("returnLoanOrders", paramDictionary);
            if (resultContainer != null)
            {
                var offers = resultContainer["offers"];
                var result = new List<LoanOffer>();
                foreach (JToken offer in offers)
                {
                    var rate = Decimal.Parse(offer["rate"].ToString());
                    var quantity = (CoinAmount)Decimal.Parse(offer["amount"].ToString());
                    var minLength = new TimeSpan((int)offer["rangemin"], 0, 0, 0);
                    var maxLength = new TimeSpan((int)offer["rangemax"], 0, 0, 0);
                    result.Add(new LoanOffer(rate, quantity, minLength, maxLength));
                }
                return result;
            }
            return null;
        }
        private (List<(Price,CoinAmount)>, List<(Price, CoinAmount)>) ParseOrderBook(JContainer container)
        {
            var askList = new List<(Price, CoinAmount)>();
            var asks = container["asks"];

            foreach (var ask in asks)
            {
                if (!decimal.TryParse(ask[0].ToString(), out decimal priceDecimal))
                {
                    priceDecimal = decimal.Parse(ask[0].ToString(), System.Globalization.NumberStyles.Float);
                }
                if (!decimal.TryParse(ask[1].ToString(), out decimal coinDecimal))
                {
                    coinDecimal = decimal.Parse(ask[1].ToString(), System.Globalization.NumberStyles.Float);
                }
                askList.Add(((Price)priceDecimal, (CoinAmount)coinDecimal));
            }
            var bidList = new List<(Price, CoinAmount)>();
            var bids = container["bids"];
            foreach (var bid in bids)
            {
                if (!decimal.TryParse(bid[0].ToString(), out decimal priceDecimal))
                {
                    priceDecimal = decimal.Parse(bid[0].ToString(), System.Globalization.NumberStyles.Float);
                }
                if (!decimal.TryParse(bid[1].ToString(), out decimal coinDecimal))
                {
                    coinDecimal = decimal.Parse(bid[1].ToString(), System.Globalization.NumberStyles.Float);
                }
                bidList.Add(((Price)priceDecimal, (CoinAmount)coinDecimal));
            }
            return (askList, bidList);
        }

        public override async ValueTask<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)> GetMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var buyMarkets = new Dictionary<CoinType, ICollection<CoinType>>();
                var sellMarkets = new Dictionary<CoinType, ICollection<CoinType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CoinInfo.GetCoinType(pair[0]);
                        var stock = CoinInfo.GetCoinType(pair[1]);
                        if (sellMarkets.ContainsKey(stock))
                        {
                            sellMarkets[stock].Add(currency);
                        }
                        else
                        {
                            sellMarkets.Add(stock, new HashSet<CoinType>() { currency });
                        }

                        if (buyMarkets.ContainsKey(currency))
                        {
                            buyMarkets[currency].Add(stock);
                        }
                        else
                        {
                            buyMarkets.Add(currency, new HashSet<CoinType>() { stock });
                        }
                    }
                }
                return (buyMarkets, sellMarkets);
            }
            return (null,null);
        }

        public override async ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var result = new Dictionary<CoinType, ICollection<CoinType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CoinInfo.GetCoinType(pair[0]);
                        var stock = CoinInfo.GetCoinType(pair[1]);
                        if (result.ContainsKey(stock))
                        {
                            result[stock].Add(currency);
                        }
                        else
                        {
                            result.Add(stock, new HashSet<CoinType>() { currency });
                        }
                    }
                }
                return result;
            }
            return null;    
        }

        public override async ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetSellMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var result = new Dictionary<CoinType, ICollection<CoinType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CoinInfo.GetCoinType(pair[0]);
                        var stock = CoinInfo.GetCoinType(pair[1]);
                        if (result.ContainsKey(currency))
                        {
                            result[currency].Add(stock);
                        }
                        else
                        {
                            result.Add(currency, new HashSet<CoinType>() { stock });
                        }
                    }
                }
                return result;
            }
            return null;
        }
        private string GetCurrencyPair(CoinType stockType, CoinType currencyType)
        {
            return $"{CoinInfo.GetDefaultAbbreviation(currencyType)}_{CoinInfo.GetDefaultAbbreviation(stockType)}";
        }

 
        
        private async ValueTask<T> SendCommand<T>(string method, Dictionary<string, string> paramDictionary = null) where T : class
        {
            return JsonConvert.DeserializeObject<T>(await this.SendCommand(method, paramDictionary) ?? "") as T;
        }
        private async ValueTask<string> SendCommand(string method, Dictionary<string, string> paramDictionary = null)
        {            
            if (paramDictionary == null)
            {
                paramDictionary = new Dictionary<string, string>();
            }
            switch (method)
            {
                case "returnBalances":
                case "buy":
                case "sell":
                case "withdraw":
                case "returnFeeInfo":
                    {
                        if (this.privateCaller != null)
                        {
                            return await this.privateCaller.MakeCall(method, paramDictionary);                            
                        }
                        throw new AccessViolationException("Cannot access these methods without providing a keypair");
                    }
                case "returnTicker":
                case "return24hVolume":
                case "returnOrderBook":
                case "returnLoanOrders":
                case "returnTradeHistory":
                    {
                        return await this.publicCaller.MakeCall(method, paramDictionary);
                    }
                default:
                    throw new ArgumentException("Don't know if the requested method is a public or private api call:" + method);
            }
        }        
    }
}