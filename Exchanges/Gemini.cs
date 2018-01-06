using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyCore.Address;
using CurrencyCore.Coin;
using ExchangesCore;
using ExchangesCore.ExchangeAPICallers;
using Logging;
using System.Collections.Concurrent;
using System;
using Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Core.Functional;

namespace Exchanges
{
    public class Gemini : ExchangeBase
    {        
        private const string Url = " https://api.gemini.com/v1/";
        private readonly ApiCaller privateCaller;
        private readonly ApiCaller publicCaller;
        private static readonly ConcurrentQueue<Action> PublicRequestQueue = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<Action> PrivateRequestQueue = new ConcurrentQueue<Action>();

        private static Task CurrentPublicRequest;
        private static Task CurrentPrivateRequest;

        static Gemini()
        {
            System.Timers.Timer timer = new System.Timers.Timer()
            {
                AutoReset = true,
                //Rate has to be less than 1 per second on average
                Interval = (1000.0 * 1.0) / 2.0
            };
            timer.Elapsed += SendPublicMessage;
            timer.Start();

            System.Timers.Timer timer2 = new System.Timers.Timer()
            {
                AutoReset = true,
                //Rate has to be less than 5 per second
                Interval = (1000.0 * 1.0) / 5.0
            };
            timer2.Elapsed += SendPrivateMessage;
            timer2.Start();
        }

        private static void SendPrivateMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentPrivateRequest == null || CurrentPrivateRequest.IsCompleted)
            {
                lock (PrivateRequestQueue)
                {
                    if (PrivateRequestQueue.TryDequeue(out var request))
                    {
                        CurrentPrivateRequest = Task.Run(request);
                    }
                }
            }
        }

        private static void SendPublicMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentPublicRequest == null || CurrentPublicRequest.IsCompleted)
            {
                lock (PublicRequestQueue)
                {
                    if (PublicRequestQueue.TryDequeue(out var request))
                    {
                        CurrentPublicRequest = Task.Run(request);
                    }
                }
            }
        }

        public Gemini(ILogger logger, KeyPair<string, string> apiKeys = null) : base(nameof(Gemini))
        {
            if (apiKeys != null)
            {
               // this.privateCaller = new PrivateCaller(new Uri(Url), logger, apiKeys, RequestQueue);
            }
            this.publicCaller = new SimpleGetCaller(new Uri(Url), logger, PublicRequestQueue);
        }

        public override ValueTask<bool> ExecuteTrade(Trade trade, Price price, CurrencyAmount amount)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<CurrencyAmount>> GetBalance(CurrencyType coinType)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<Dictionary<CurrencyType, CurrencyAmount>>> GetBalances(bool ignoreCache = false)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetBuyMarkets()
        {
            throw new NotImplementedException();
        }

        public override ValueTask<(Maybe<Fee> maker, Maybe<Fee> taker)> GetCurrentTradeFees()
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<List<LoanOffer>>> GetLoanOrderBook(CurrencyType currencyType, int depth = 50)
        {
            throw new NotImplementedException();
        }

        public async override ValueTask<(Dictionary<CurrencyType, ICollection<CurrencyType>> buyMarkets, Dictionary<CurrencyType, ICollection<CurrencyType>> sellMarkets)> GetMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("symbols");

            var buyMarkets = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
            var sellMarkets = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
            return resultContainer.Case(
                some: (value) =>
                 {
                     foreach (JToken currencyPair in value)
                     {
                         if (currencyPair.Value<string>().Length == 6)
                         {
                             var currency = CryptoCurrency.GetCurrencyType(currencyPair.Value<string>().Substring(0, 3));
                             var stock = CryptoCurrency.GetCurrencyType(currencyPair.Value<string>().Substring(3, 3));
                             if (buyMarkets.ContainsKey(stock))
                             {
                                 buyMarkets[stock].Add(currency);
                             }
                             else
                             {
                                 buyMarkets.Add(stock, new HashSet<CurrencyType>() { currency });
                             }
                             if (sellMarkets.ContainsKey(currency))
                             {
                                 sellMarkets[currency].Add(stock);
                             }
                             else
                             {
                                 sellMarkets.Add(currency, new HashSet<CurrencyType>() { stock });
                             }
                         }
                     }
                     return (buyMarkets, sellMarkets);
                 },
                none: () => (new Dictionary<CurrencyType, ICollection<CurrencyType>>(), new Dictionary<CurrencyType, ICollection<CurrencyType>>()));       
        }

        public async override ValueTask<(Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)> GetOrderBook(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            var paramDictionary = new Dictionary<string, string>
                 {
                    {"limit_bids", depth.ToString()},
                    {"limit_asks", depth.ToString()}
                 };
            var resultContainer = await this.SendCommand<JContainer>("book", $"{Gemini.GetCurrencyAbbreviation(stockType)}{Gemini.GetCurrencyAbbreviation(currencyType)}", paramDictionary);

            return resultContainer.Case(
                some: (value) =>
                {
                    var askResult = Maybe<List<(Price, CurrencyAmount)>>.None;
                    var askList = new List<(Price, CurrencyAmount)>();


                    var asks = value["asks"];

                    foreach (var ask in asks)
                    {
                        if (!decimal.TryParse(ask["price"].ToString(), out decimal priceDecimal))
                        {
                            priceDecimal = decimal.Parse(ask["price"].ToString(), System.Globalization.NumberStyles.Float);
                        }
                        if (!decimal.TryParse(ask["amount"].ToString(), out decimal coinDecimal))
                        {
                            coinDecimal = decimal.Parse(ask["amount"].ToString(), System.Globalization.NumberStyles.Float);
                        }
                        askList.Add(((Price)priceDecimal, (CurrencyAmount)coinDecimal));
                    }
                    if (askList.Count > 0)
                    {
                        askResult = Maybe.Some(askList);
                    }
                    var bidResult = Maybe<List<(Price, CurrencyAmount)>>.None;
                    var bidList = new List<(Price, CurrencyAmount)>();
                    var bids = value["bids"];
                    foreach (var bid in bids)
                    {
                        if (!decimal.TryParse(bid["price"].ToString(), out decimal priceDecimal))
                        {
                            priceDecimal = decimal.Parse(bid["price"].ToString(), System.Globalization.NumberStyles.Float);
                        }
                        if (!decimal.TryParse(bid["amount"].ToString(), out decimal coinDecimal))
                        {
                            coinDecimal = decimal.Parse(bid["amount"].ToString(), System.Globalization.NumberStyles.Float);
                        }
                        bidList.Add(((Price)priceDecimal, (CurrencyAmount)coinDecimal));
                    }
                    if (bidList.Count > 0)
                    {
                        bidResult = Maybe.Some(bidList);
                    }
                    return (askResult, bidResult);
                },
                none: () => (Maybe<List<(Price, CurrencyAmount)>>.None, Maybe<List<(Price, CurrencyAmount)>>.None));
        }

        public override ValueTask<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>> GetOrderBooks(int depth = 50)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetSellMarkets()
        {
            throw new NotImplementedException();
        }

        public async override ValueTask<Maybe<List<CompletedTrade>>> GetTradeHistory(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            var paramDictionary = new Dictionary<string, string>
                 {
                    {"limit_trades", depth.ToString()}
                 };
            var resultContainer = await this.SendCommand<JContainer>("trades", $"{Gemini.GetCurrencyAbbreviation(stockType)}{Gemini.GetCurrencyAbbreviation(currencyType)}", paramDictionary);
            List<CompletedTrade> result = null;
            return resultContainer.Case(some:
            (value) =>
            {
                result = new List<CompletedTrade>();
                foreach (var trade in value)
                {
                    var type = trade.Value<string>("type").Equals("sell") ? TradeType.Sell : (trade.Value<string>("type").Equals("buy")? TradeType.Buy : TradeType.Auction);
                    var date = DateTimeOffset.FromUnixTimeSeconds(trade.Value<long>("timestamp")).UtcDateTime;
                    var rate = (Price)trade.Value<decimal>("price");
                    var amount = (CurrencyAmount)trade.Value<decimal>("amount");
                    result.Add(new CompletedTrade(type, stockType, currencyType, rate, amount, date));
                }
                return Maybe.Some(result);
            },
            none: () => Maybe<List<CompletedTrade>>.None);
        }

        public override ValueTask<string> Withdraw(PublicAddress address, CurrencyAmount amount)
        {
            throw new NotImplementedException();
        }

        private static string GetCurrencyAbbreviation(CurrencyType coinType)
        {
            switch (coinType)
            {
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

        private async ValueTask<Maybe<T>> SendCommand<T>(string method, string symbol = null, Dictionary < string, string> paramDictionary = null) where T : class
        {
            var result = JsonConvert.DeserializeObject<T>(await this.SendCommand(method, symbol, paramDictionary) ?? "") as T;
            return result != null ? Maybe.Some(result) : Maybe<T>.None; 
        }
        private async ValueTask<string> SendCommand(string method, string symbol = null, Dictionary<string, string> paramDictionary = null)
        {
            if (paramDictionary == null)
            {
                paramDictionary = new Dictionary<string, string>();
            }
            switch (method)
            {
                case "order":
                case "buy":
                case "sell":
                case "withdraw":
                case "returnFeeInfo":
                    {
                        if (this.privateCaller != null)
                        {
                            return await this.privateCaller.MakeCall($"{method}/{symbol}", paramDictionary);
                        }
                        throw new AccessViolationException("Cannot access these methods without providing a keypair");
                    }
                case "symbols":
                    {
                        return await this.publicCaller.MakeCall($"{method}", paramDictionary);
                    }
                case "pubticker":
                case "book":
                case "trades":
                case "auction":
                    {
                        var anyParams = paramDictionary.Count > 0 ? "?" : "";
                        return await this.publicCaller.MakeCall($"{method}/{symbol}{anyParams}", paramDictionary);
                    }
                case "auctionhistory":
                    {
                        return await this.publicCaller.MakeCall($"auction/{symbol}/history", paramDictionary);
                    }
                default:
                    throw new ArgumentException("Don't know if the requested method is a public or private api call:" + method);
            }
        }       
    }    
}
