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
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Security.Cryptography;

    public class Bittrex : ExchangeBase
    {
        private static readonly ConcurrentQueue<Action> RequestQueue = new ConcurrentQueue<Action>();

        private static Task CurrentRequest;
        static Bittrex()
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

        private const string BaseUrl = "https://bittrex.com/api/v1.1/";
        private readonly ApiCaller accountCaller;
        private readonly ApiCaller publicCaller;
        private readonly ApiCaller marketCaller;

        public Bittrex(ILogger logger, KeyPair<string, string> apiKeys = null) : base(nameof(Bittrex))
        {
            if (apiKeys != null)
            {
                this.accountCaller = new PrivateCaller(new Uri(BaseUrl + "account/"), logger, apiKeys, RequestQueue);
                this.marketCaller = new PrivateCaller(new Uri(BaseUrl + "market/"), logger, apiKeys, RequestQueue);
            }
            this.publicCaller = new SimpleGetCaller(new Uri(BaseUrl + "public/"), logger, RequestQueue);
        }

        public override async ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees()
        {
            return await Task.FromResult((new Fee(0.0025M, CoinAmount.Zero), new Fee(0.0025M, CoinAmount.Zero)));
        }

        public override async ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount)
        {
            var command = (trade.Type == TradeType.Buy ? "buy" : "sell")+"limit";
            var paramDictionary = new Dictionary<string, string>
            {
               {"market", $"{this.GetCurrencyPair(trade.StockCoin, trade.CurrencyCoin)}" },
               {"quantity", amount.ToString()},
               {"rate", price.ToString()},
            };

            var resultContainer = await this.SendCommand<JContainer>(command, paramDictionary);
            return true;
        }
        
        public override async ValueTask<CoinAmount?> GetBalance(CoinType coinType)
        {
            return CoinAmount.Zero;
        }

        public override async ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances(bool ignoreCache = false)
        {
            return await this.GetValue<Dictionary<CoinType, CoinAmount>>("Balances", async () =>
            {
                var jsonResult = (await this.SendCommand<JContainer>("getbalances"))["result"];
                if (jsonResult != null)
                {
                    var result = new Dictionary<CoinType, CoinAmount>();
                    foreach (var coinEntry in jsonResult)
                    {
                        var coinType = CoinInfo.GetCoinType(coinEntry.Value<string>("currency"));

                        if (!result.ContainsKey(coinType))
                        {
                            result.Add(coinType, (CoinAmount)coinEntry.Value<decimal>("available"));
                        }
                    }
                    return result;
                }
                return null;
            },
            secondsTilExpiration: 60);
        }

        public override async ValueTask<string> Withdraw(PublicAddress address, CoinAmount amount)
        {
            return null;
        }

        public override async ValueTask<List<CompletedTrade>> GetTradeHistory(CoinType stockType, CoinType currencyType, int depth = 50)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "TradeHistory:" + currencyPair;
            var paramDictionary = new Dictionary<string, string>
            {
                {"market", currencyPair},
            };
            return await this.GetValue<List<CompletedTrade>>(key, async () =>
            {
                var jsonResult = (await this.SendCommand<JContainer>("getmarkethistory", paramDictionary))["result"];
                if (jsonResult != null)
                {
                    List<CompletedTrade> result = null;
                    result = new List<CompletedTrade>();
                    foreach (var trade in jsonResult)
                    {
                        var type = trade.Value<string>("ordertype").Equals("sell") ? TradeType.Sell : TradeType.Buy;
                        var date = trade.Value<DateTime>("timestamp");
                        var rate = (Price)trade.Value<decimal>("price");
                        var amount = (CoinAmount)trade.Value<decimal>("quantity");
                        result.Add(new CompletedTrade(type, stockType, currencyType, rate, amount, date));
                    }
                    return result;
                }
                return null;
            },
         secondsTilExpiration: 60);

        }

        public override async ValueTask<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>> GetOrderBooks(int depth = 50)
        {
            return null;
        }

        public override async ValueTask<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)> GetOrderBook(CoinType stockType, CoinType currencyType, int depth = 20)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "orderbook:" + currencyPair;
            var paramDictionary = new Dictionary<string, string>
            {
                {"market", currencyPair},
                {"type", "both"},
                {"depth", depth.ToString()},
            };
            return await this.GetValue<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>(key, async () =>
            {
                var jsonResult = (await this.SendCommand<JContainer>("getorderbook", paramDictionary))["result"];
                if (jsonResult != null)
                {
                    List<(Price, CoinAmount)> askList = new List<(Price, CoinAmount)>(), bidList = new List<(Price, CoinAmount)>();

                    var asks = jsonResult["sell"];
                    foreach (var ask in asks)
                    {
                        askList.Add(((Price)ask.Value<decimal>("rate"), (CoinAmount)ask.Value<decimal>("quantity")));
                    }
                    var bids = jsonResult["buy"];
                    foreach (var bid in bids)
                    {
                        bidList.Add(((Price)bid.Value<decimal>("rate"), (CoinAmount)bid.Value<decimal>("quantity")));
                    }
                    return (askList, bidList);
                }
                return (null,null);
            },
         secondsTilExpiration: 60);
        }

        public override async ValueTask<List<LoanOffer>> GetLoanOrderBook(CoinType currencyType, int depth = 50)
        {
            return null;
        }
        private (List<(Price, CoinAmount)>, List<(Price, CoinAmount)>) ParseOrderBook(JContainer container)
        {
            return (null, null);
        }

        public override async ValueTask<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)> GetMarkets()
        {
            return await this.GetValue<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)>("Markets", async () =>
            {
                var jsonResult = (await this.SendCommand<JContainer>("getmarkets"))["result"];
                if (jsonResult != null)
                {
                    var buyMarkets = new Dictionary<CoinType, ICollection<CoinType>>();
                    var sellMarkets = new Dictionary<CoinType, ICollection<CoinType>>();
                    foreach (JToken marketData in jsonResult)
                    {
                        var currency = CoinInfo.GetCoinType(marketData.Value<string>("basecurrency"));
                        var stock = CoinInfo.GetCoinType(marketData.Value<string>("marketcurrency"));
                        if (marketData.Value<bool>("isactive"))
                        {
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
                return (null, null);
            },
           secondsTilExpiration: 60);
        }

        public override async ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets()
        {
            return null;
        }

        public override async ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetSellMarkets()
        {
           return null;
        }
        private string GetCurrencyPair(CoinType stockType, CoinType currencyType)
        {
            
            return $"{GetCurrencyAbbreviation(currencyType)}-{GetCurrencyAbbreviation(stockType)}";
        }
        private static string GetCurrencyAbbreviation(CoinType coinType)
        {
            switch (coinType)
            {
                case CoinType.None:
                case CoinType.Bitcoin:
                case CoinType.Dash:
                case CoinType.DigiByte:
                case CoinType.Ethereum:
                case CoinType.EthereumClassic:
                case CoinType.Factom:
                case CoinType.Litecoin:
                case CoinType.Monero:
                case CoinType.Ripple:   
                case CoinType.Stellar:
                case CoinType.USDTether:
                case CoinType.Vericoin:
                case CoinType.Verium:
                case CoinType.ZCash:
                case CoinType.BitcoinCash:
                    {
                        return CoinInfo.GetDefaultAbbreviation(coinType);
                    }
            }
            return "";
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
                case "getbalances":
                case "getbalance":
                case "getdepositaddress":
                case "withdraw":
                case "getorder":
                case "getorderhistory":
                case "getwithdrawalhistory":
                case "getdeposithistory":
                    {
                        if (this.accountCaller != null)
                        {
                            return await this.accountCaller.MakeCall(method, paramDictionary);
                        }
                        throw new AccessViolationException("Cannot access these methods without providing a keypair");
                    }
                case "getopenorders":
                case "cancel":
                case "selllimit":
                case "buylimit":
                    {
                        if (this.marketCaller != null)
                        {
                            return await this.marketCaller.MakeCall(method, paramDictionary);
                        }
                        throw new AccessViolationException("Cannot access these methods without providing a keypair");
                    }
                case "getmarkets":
                case "getcurrencies":
                case "getticker":
                case "getmarketsummaries":
                case "getmarketsummary":
                case "getorderbook":
                case "getmarkethistory":
                    {
                        method = method + (paramDictionary.Count > 0 ? "?" : "");
                        return await this.publicCaller.MakeCall(method, paramDictionary);
                    }
                default:
                    throw new ArgumentException("Don't know if the requested method is a public or private api call:" + method);
            }
        }
        
        private class PrivateCaller : SignedPostCaller
        {
            public PrivateCaller(Uri uri, ILogger logger, KeyPair<string, string> keys, ConcurrentQueue<Action> requestQueue) : base(uri, logger,keys, requestQueue)
            {
            }
         
            /// Call an api method that has parameters.
            /// </summary>
            /// <param name="method">The name of the method you wish to call.</param>
            /// <param name="paramDictionary">
            /// A dictionary that has key/value pairs representing the paramaters you wish to pass to the
            /// Cryptsy method.
            /// </param>
            /// <returns>The string returned by the call to the api or null if there was a failure. </returns>
            protected override WebRequest GetWebRequest(
                string method,
                Dictionary<string, string> paramDictionary)
            {
                var uriString = $"{ this.Uri}{method}?apikey={this.keys.PublicKey}&nonce={ DateTime.UtcNow.Ticks / 100}";
                uriString = paramDictionary.Keys.Aggregate(
                    uriString,
                    (current, key) => current + ("&" + key + "=" + paramDictionary[key]));
                var uri = new Uri(uriString);
                
                return this.CreateRequest(uri, uriString);             
            }
            protected override void AddSignature(HttpWebRequest request, string signature)
            {
                request.Headers.Add("apisign", signature);
            }
        }
    }
}