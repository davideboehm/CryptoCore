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
    using System.Net;
    using System.Linq;
    using System.Collections.Specialized;
    using Core.Functional;

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
        private readonly ApiCaller privateCaller;
        private readonly ApiCaller publicCaller;

        public Poloniex(ILogger logger, KeyPair<string, string> apiKeys = null) : base(nameof(Poloniex))
        {
            if (apiKeys != null)
            {
                this.privateCaller = new PrivateCaller(new Uri(PrivateUrl), logger, apiKeys, RequestQueue);
            }
            this.publicCaller = new SimpleGetCaller(new Uri(PublicUrl), logger, RequestQueue);
        }

        public override async ValueTask<(Maybe<Fee> maker, Maybe<Fee> taker)> GetCurrentTradeFees()
        {
            var resultContainer = await this.SendCommand<JContainer>("returnFeeInfo");

            if (resultContainer != null)
            {
                var maker = new Fee(resultContainer.Value<Decimal>("makerfee"), CurrencyAmount.Zero);
                var taker = new Fee(resultContainer.Value<Decimal>("takerfee"), CurrencyAmount.Zero);
                return (Maybe.Some(maker), Maybe.Some(taker));
            }

            return (Maybe<Fee>.None, Maybe<Fee>.None);
        }
        public override async ValueTask<bool> ExecuteTrade(Trade trade, Price price, CurrencyAmount amount)
        {
            return await this.ExecuteTrade(trade, price, amount, false);
        }

        public async ValueTask<bool> ExecuteTrade(Trade trade, Price price, CurrencyAmount amount, bool postOnly)
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

        public override async ValueTask<Maybe<CurrencyAmount>> GetBalance(CurrencyType coinType)
        {
            var balances = await this.GetBalances();
            return balances.Case(
                some: (dict) => dict.TryGetValue(coinType, out var result) ? Maybe.Some(result) : Maybe<CurrencyAmount>.None);
        }

        public override async ValueTask<Maybe<Dictionary<CurrencyType, CurrencyAmount>>> GetBalances(bool ignoreCache = false)
        {
            async ValueTask<Maybe<Dictionary<CurrencyType, CurrencyAmount>>> getBalances()
            {
                var jsonResult = await this.SendCommand<Dictionary<string, decimal>>("returnBalances");
                if (jsonResult != null)
                {
                    var result = new Dictionary<CurrencyType, CurrencyAmount>();
                    foreach (var key in jsonResult.Keys)
                    {
                        var coinType = CryptoCurrency.GetCurrencyType(key);
                        if (!result.ContainsKey(coinType))
                        {
                            result.Add(coinType, new CurrencyAmount(jsonResult[key], coinType));
                        }
                    }
                    return Maybe<Dictionary<CurrencyType, CurrencyAmount>>.Some(result);
                }
                return Maybe<Dictionary<CurrencyType, CurrencyAmount>>.None;
            }

            if (ignoreCache)
            {
                var result = await getBalances();
            }

            return await this.GetValue<Maybe<Dictionary<CurrencyType, CurrencyAmount>>>("Balances", async () =>
            {
                return await getBalances();
            },
            secondsTilExpiration: 60);
        }

        public override async ValueTask<string> Withdraw(PublicAddress address, CurrencyAmount amount)
        {
            var paramDictionary = new Dictionary<string, string>
            {
               {"currency", CryptoCurrency.GetDefaultAbbreviation(address.AddressType)},
               {"amount", amount.ToString()},
               {"address", address.ToString()}
            };

            return await this.SendCommand("withdraw", paramDictionary);
        }

        public override async ValueTask<Maybe<List<CompletedTrade>>> GetTradeHistory(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "TradeHistory:" + currencyPair;

            return await this.GetValue<Maybe<List<CompletedTrade>>>(key, async () =>
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
                       var type = trade.Value<string>("type").Equals("sell") ? TradeType.Sell : TradeType.Buy;
                       var date = trade.Value<DateTime>("date");
                       var rate = new Price(trade.Value<decimal>("rate"), stockType, currencyType);
                       var amount = new CurrencyAmount(trade.Value<decimal>("amount"), stockType);
                       result.Add(new CompletedTrade(type, stockType, currencyType, rate, amount, date));
                   }
                   return Maybe.Some(result);
               }
               return Maybe<List<CompletedTrade>>.None;
           },
            secondsTilExpiration: 20);
        }
                
        public override async ValueTask<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>> GetOrderBooks(int depth = 50)
        {
            return await this.GetValue<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>>("orderbook", async () =>
            {
                var result = new Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>(StringComparer.InvariantCultureIgnoreCase);
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
                        var pair = market.Key.Split('_');
                        if (pair.Length == 2)
                        {
                            var currency = Currency.GetCurrencyType(pair[0]);
                            var stock = Currency.GetCurrencyType(pair[1]);
                            var data = ParseOrderBook(market.Value, stock, currency);
                            if (data.Item1.HasValue() && data.Item2.HasValue())
                            {
                                result.Add(market.Key, (data.Item1.Value(), data.Item2.Value()));
                            }
                        }
                    }
                    return Maybe.Some(result);
                }
                return Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>.None;
            }
            );
        }

        public override async ValueTask<(Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)> GetOrderBook(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            var currencyPair = this.GetCurrencyPair(stockType, currencyType);
            var key = "OrderBook:" + currencyPair;
            return await this.GetValue<(Maybe<List<(Price, CurrencyAmount)>>, Maybe<List<(Price, CurrencyAmount)>>)>(key, async () =>
        {
                //try to grab the full order dictionary from the cache
                var fullOrderDictionary = await this.GetValue<Dictionary<string, (Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)>>
            ("orderbook",
            async () =>
            {
                    //no op
                    return await Task.FromResult<Dictionary<string, (Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)>>(null);
            });

            if (fullOrderDictionary != null && fullOrderDictionary.TryGetValue(currencyPair, out var cachedResult))
            {
                return cachedResult;
            }

                //there is no cached result so try to get the individual orderbook
                var paramDictionary = new Dictionary<string, string>
            {
                    {"currencyPair", currencyPair },
                    {"depth", depth.ToString()}
            };

            (Maybe<List<(Price, CurrencyAmount)>>, Maybe<List<(Price, CurrencyAmount)>>) book = (Maybe<List<(Price, CurrencyAmount)>>.None, Maybe<List<(Price, CurrencyAmount)>>.None);

            var response = await this.SendCommand<JContainer>("returnOrderBook", paramDictionary);

            if (response != null)
            {
                book = ParseOrderBook(response, stockType, currencyType);
            }
            return book;
        });
        }

        public override async ValueTask<Maybe<List<LoanOffer>>> GetLoanOrderBook(CurrencyType currencyType, int depth = 50)
        {
            var paramDictionary = new Dictionary<string, string>
            {
                {"currency", $"{Currency.GetDefaultAbbreviation(currencyType)}" },
                {"limit", depth.ToString()}
            };

            var resultContainer = await this.SendCommand<JContainer>("returnLoanOrders", paramDictionary);
            if (resultContainer != null)
            {
                var offers = resultContainer["offers"];
                var result = new List<LoanOffer>();
                foreach (JToken offer in offers)
                {
                    var rate = offer.Value<Decimal>("rate");
                    var quantity = new CurrencyAmount(offer.Value<Decimal>("amount"), currencyType);
                    var minLength = new TimeSpan(offer.Value<int>("rangemin"), 0, 0, 0);
                    var maxLength = new TimeSpan(offer.Value<int>("rangemax"), 0, 0, 0);
                    result.Add(new LoanOffer(rate, quantity, minLength, maxLength));
                }
                return Maybe.Some(result);
            }
            return Maybe<List<LoanOffer>>.None;
        }

        private (Maybe<List<(Price, CurrencyAmount)>>, Maybe<List<(Price, CurrencyAmount)>>) ParseOrderBook(JContainer container, CurrencyType stockType, CurrencyType currencyType)
        {
            if (container.Value<int>("isfrozen") == 0)
            {
                var askResult = Maybe<List<(Price, CurrencyAmount)>>.None;
                var askList = new List<(Price, CurrencyAmount)>();
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
                    askList.Add((new Price(priceDecimal, stockType, currencyType), new CurrencyAmount(coinDecimal, stockType)));
                }
                if (askList.Count > 0)
                {
                    askResult = Maybe.Some(askList);
                }
                var bidResult = Maybe<List<(Price, CurrencyAmount)>>.None;
                var bidList = new List<(Price, CurrencyAmount)>();
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
                    bidList.Add((new Price(priceDecimal, stockType, currencyType), new CurrencyAmount(coinDecimal, stockType)));
                }
                if (bidList.Count > 0)
                {
                    bidResult = Maybe.Some(bidList);
                }
                return (askResult, bidResult);
            }
            return (Maybe<List<(Price, CurrencyAmount)>>.None, Maybe<List<(Price, CurrencyAmount)>>.None);
        }

        public override async ValueTask<(Dictionary<CurrencyType, ICollection<CurrencyType>> buyMarkets, Dictionary<CurrencyType, ICollection<CurrencyType>> sellMarkets)> GetMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var buyMarkets = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
                var sellMarkets = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = Currency.GetCurrencyType(pair[0]);
                        var stock = Currency.GetCurrencyType(pair[1]);
                        if (sellMarkets.ContainsKey(stock))
                        {
                            sellMarkets[stock].Add(currency);
                        }
                        else
                        {
                            sellMarkets.Add(stock, new HashSet<CurrencyType>() { currency });
                        }

                        if (buyMarkets.ContainsKey(currency))
                        {
                            buyMarkets[currency].Add(stock);
                        }
                        else
                        {
                            buyMarkets.Add(currency, new HashSet<CurrencyType>() { stock });
                        }
                    }
                }
                return (buyMarkets, sellMarkets);
            }
            return (null, null);
        }

        public override async ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetBuyMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var result = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CryptoCurrency.GetCurrencyType(pair[0]);
                        var stock = CryptoCurrency.GetCurrencyType(pair[1]);
                        if (result.ContainsKey(stock))
                        {
                            result[stock].Add(currency);
                        }
                        else
                        {
                            result.Add(stock, new HashSet<CurrencyType>() { currency });
                        }
                    }
                }
                return Maybe.Some(result);
            }
            return Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>.None;
        }

        public async ValueTask<Maybe<decimal>> Get24HourChange(CurrencyType stock, CurrencyType currency)
        {
            var data = await this.GetTickerData();
            return data.Case(
                some: (value) =>
                 {
                     if (value.TryGetValue((stock, currency), out var tickerData))
                     {
                         return Maybe.Some(tickerData.percentChange);
                     }
                     return Maybe<decimal>.None;
                 });
        }

        private async ValueTask<Maybe<Dictionary<(CurrencyType stock, CurrencyType currency), TickerData>>> GetTickerData()
        {
            var resultContainer = await this.SendCommand<JContainer>("returnTicker");
            if (resultContainer != null)
            {
                var result = new Dictionary<(CurrencyType stock, CurrencyType currency), TickerData>();
                foreach (var currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CryptoCurrency.GetCurrencyType(pair[0]);
                        var stock = CryptoCurrency.GetCurrencyType(pair[1]);
                        if (stock != CurrencyType.None && currency != CurrencyType.None)
                        {
                            foreach (var data in currencyPair)
                            {
                                if (data["isfrozen"].Value<int>() == 0)
                                {
                                    result.Add((stock, currency), new TickerData(
                                        currencyType: currency,
                                        stockType: stock,
                                        lowestAsk: new Price(data["lowestask"].Value<decimal>(), stock, currency),
                                        last: new Price(data["last"].Value<decimal>(), stock, currency),
                                        highestBid: new Price(data["highestbid"].Value<decimal>(), stock, currency),
                                        percentChange: new Price(data["percentchange"].Value<decimal>(), stock, currency),
                                        baseVolume: new CurrencyAmount(data["basevolume"].Value<decimal>(), currency),
                                        quoteVolume: new CurrencyAmount(data["quotevolume"].Value<decimal>(), stock)
                                    ));
                                }
                            }
                        }
                    }
                }
                return Maybe.Some(result);
            }
            return Maybe<Dictionary<(CurrencyType stock, CurrencyType currency), TickerData>>.None;
        }


        public override async ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetSellMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("return24hVolume");
            if (resultContainer != null)
            {
                var result = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
                foreach (JToken currencyPair in resultContainer)
                {
                    var pair = currencyPair.Path.Split('_');
                    if (pair.Length == 2)
                    {
                        var currency = CryptoCurrency.GetCurrencyType(pair[0]);
                        var stock = CryptoCurrency.GetCurrencyType(pair[1]);
                        if (result.ContainsKey(currency))
                        {
                            result[currency].Add(stock);
                        }
                        else
                        {
                            result.Add(currency, new HashSet<CurrencyType>() { stock });
                        }
                    }
                }
                return Maybe.Some(result);
            }
            return Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>.None;
        }
        private string GetCurrencyPair(CurrencyType stockType, CurrencyType currencyType)
        {
            return $"{GetCurrencyAbbreviation(currencyType)}_{GetCurrencyAbbreviation(stockType)}";
        }
        private static string GetCurrencyAbbreviation(CurrencyType coinType)
        {
            switch (coinType)
            {
                case CurrencyType.None:
                    {
                        return "";
                    }
                case CurrencyType.BitcoinCash:
                    {
                        return CryptoCurrency.GetCryptoCurrency(coinType).GetAbbreviations()[1];
                    }
                default:
                    {
                        return CryptoCurrency.GetDefaultAbbreviation(coinType);
                    }
            }
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
                        return await this.publicCaller.MakeCall("?command=" + method, paramDictionary);
                    }
                default:
                    throw new ArgumentException("Don't know if the requested method is a public or private api call:" + method);
            }
        }
        private class PrivateCaller : SignedPostCaller
        {
            public PrivateCaller(Uri uri, ILogger logger, KeyPair<string, string> keys, ConcurrentQueue<Action> requestQueue) : base(uri, logger, keys, requestQueue)
            {
            }

            /// <summary>
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
                var postData = "command=" + method + "&nonce=" + (DateTime.UtcNow.Ticks / 100);
                postData = paramDictionary.Keys.Aggregate(
                    postData,
                    (current, key) => current + ("&" + key + "=" + paramDictionary[key]));

                var headers = new NameValueCollection()
                {
                    { "Key", this.keys.PublicKey.ToString() }
                };

                return this.CreateRequest(this.Uri, postData, headers);
            }
            protected override void AddSignature(HttpWebRequest request, string signature)
            {
                request.Headers.Add("Sign", signature);
            }
        }
        private struct TickerData
        {
            public CurrencyType stockType, currencyType;
            public Price last, lowestAsk, highestBid;
            public decimal percentChange;
            public CurrencyAmount baseVolume, quoteVolume;

            public TickerData(CurrencyType stockType, CurrencyType currencyType, Price last, Price lowestAsk, Price highestBid, decimal percentChange, CurrencyAmount baseVolume, CurrencyAmount quoteVolume)
            {
                this.stockType = stockType;
                this.currencyType = currencyType;
                this.last = last;
                this.lowestAsk = lowestAsk;
                this.highestBid = highestBid;
                this.percentChange = percentChange;
                this.baseVolume = baseVolume;
                this.quoteVolume = quoteVolume;
            }
        }
    }
}