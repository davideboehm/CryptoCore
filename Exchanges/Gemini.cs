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

        public override ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<CoinAmount?> GetBalance(CoinType coinType)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances(bool ignoreCache = false)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets()
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees()
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<List<LoanOffer>> GetLoanOrderBook(CoinType currencyType, int depth = 50)
        {
            throw new System.NotImplementedException();
        }

        public override async ValueTask<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)> GetMarkets()
        {
            var resultContainer = await this.SendCommand<JContainer>("symbols");
            return (null,null);
        }

        public override ValueTask<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)> GetOrderBook(CoinType stockType, CoinType currencyType, int depth = 50)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>> GetOrderBooks(int depth = 50)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetSellMarkets()
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<List<CompletedTrade>> GetTradeHistory(CoinType stockType, CoinType currencyType, int depth = 50)
        {
            throw new System.NotImplementedException();
        }

        public override ValueTask<string> Withdraw(PublicAddress address, CoinAmount amount)
        {
            throw new System.NotImplementedException();
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
                    {
                        return CoinInfo.GetDefaultAbbreviation(coinType);
                    }
                case CoinType.BitcoinCash:
                    {
                        return CoinInfo.GetCoinInfo(coinType).GetAbbreviations()[1];
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
    }    
}
