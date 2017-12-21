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


        private static string GetCurrencyAbbreviation(CurrencyType coinType)
        {
            switch (coinType)
            {
                case CurrencyType.None:
                case CurrencyType.Bitcoin:
                case CurrencyType.Dash:
                case CurrencyType.DigiByte:
                case CurrencyType.Ethereum:
                case CurrencyType.EthereumClassic:
                case CurrencyType.Factom:
                case CurrencyType.Litecoin:
                case CurrencyType.Monero:
                case CurrencyType.Ripple:
                case CurrencyType.Stellar:
                case CurrencyType.USDTether:
                case CurrencyType.Vericoin:
                case CurrencyType.Verium:
                case CurrencyType.ZCash:
                    {
                        return CryptoCurrency.GetDefaultAbbreviation(coinType);
                    }
                case CurrencyType.BitcoinCash:
                    {
                        return CryptoCurrency.GetCryptoCurrency(coinType).GetAbbreviations()[1];
                    }
            }

            return "";
        }

        private async ValueTask<T> SendCommand<T>(string method, string symbol = null, Dictionary < string, string> paramDictionary = null) where T : class
        {
            return JsonConvert.DeserializeObject<T>(await this.SendCommand(method, symbol, paramDictionary) ?? "") as T;
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
                        return await this.publicCaller.MakeCall($"{method}/{symbol}", paramDictionary);
                    }
                case "auctionhistory":
                    {
                        return await this.publicCaller.MakeCall($"auction/{symbol}/history", paramDictionary);
                    }
                default:
                    throw new ArgumentException("Don't know if the requested method is a public or private api call:" + method);
            }
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

            var result = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
            var result1 = new Dictionary<CurrencyType, ICollection<CurrencyType>>();
            foreach (JToken currencyPair in resultContainer)
            {
                if (currencyPair.Value<string>().Length == 6)
                {
                    var currency = CryptoCurrency.GetCurrencyType(currencyPair.Value<string>().Substring(0,3));
                    var stock = CryptoCurrency.GetCurrencyType(currencyPair.Value<string>().Substring(3, 3));
                    if (result.ContainsKey(stock))
                    {
                        result[stock].Add(currency);
                    }
                    else
                    {
                        result.Add(stock, new HashSet<CurrencyType>() { currency });
                    }
                    if (result1.ContainsKey(currency))
                    {
                        result1[currency].Add(stock);
                    }
                    else
                    {
                        result1.Add(currency, new HashSet<CurrencyType>() { stock });
                    }
                }
            }
            return (result, result1);
        }

        public override ValueTask<(Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)> GetOrderBook(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>> GetOrderBooks(int depth = 50)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetSellMarkets()
        {
            throw new NotImplementedException();
        }

        public override ValueTask<Maybe<List<CompletedTrade>>> GetTradeHistory(CurrencyType stockType, CurrencyType currencyType, int depth = 50)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<string> Withdraw(PublicAddress address, CurrencyAmount amount)
        {
            throw new NotImplementedException();
        }
    }    
}
