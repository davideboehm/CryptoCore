using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyCore.Address;
using CurrencyCore.Coin;
using System.Runtime.Caching;
using System.Threading;
using Core.Functional;

namespace ExchangesCore
{
    public abstract class ExchangeBase : IExchange
    {
        private MemoryCache dataCache;
        private Dictionary<string, AutoResetEvent> dataCacheLocks = new Dictionary<string, AutoResetEvent>();

        public abstract ValueTask<bool> ExecuteTrade(Trade trade, Price price, CurrencyAmount amount);
        public abstract ValueTask<Maybe<CurrencyAmount>> GetBalance(CurrencyType coinType);
        public abstract ValueTask<Maybe<Dictionary<CurrencyType, CurrencyAmount>>> GetBalances(bool ignoreCache = false);
        public abstract ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetBuyMarkets();
        public abstract ValueTask<(Maybe<Fee> maker, Maybe<Fee> taker)> GetCurrentTradeFees();
        public abstract ValueTask<Maybe<List<LoanOffer>>> GetLoanOrderBook(CurrencyType currencyType, int depth = 50);
        public abstract ValueTask<(Dictionary<CurrencyType, ICollection<CurrencyType>> buyMarkets, Dictionary<CurrencyType, ICollection<CurrencyType>> sellMarkets)> GetMarkets();
        public abstract ValueTask<(Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)> GetOrderBook(CurrencyType stockType, CurrencyType currencyType, int depth = 50);
        public abstract ValueTask<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>> GetOrderBooks(int depth = 50);
        public abstract ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetSellMarkets();
        public abstract ValueTask<Maybe<List<CompletedTrade>>> GetTradeHistory(CurrencyType stockType, CurrencyType currencyType, int depth = 50);
        public abstract ValueTask<string> Withdraw(PublicAddress address, CurrencyAmount amount);

        protected ExchangeBase(string className)
        {
            this.dataCache = new MemoryCache(className + Thread.CurrentThread.ManagedThreadId);
        }

        public virtual async ValueTask<(Maybe<PriceRange> sellPrice, Maybe<PriceRange> buyPrice)> GetEstimatedExchangeRates(CurrencyType stockCoin, CurrencyType currencyCoin)
        {
            var history = await this.GetTradeHistory(stockCoin, currencyCoin);

            var (asks, bids) = await this.GetOrderBook(stockCoin, currencyCoin);
            return this.GetEstimatedExchangeRate(history, bids, asks);
        }

        protected (Maybe<PriceRange>, Maybe<PriceRange>) GetEstimatedExchangeRate(Maybe<List<CompletedTrade>> history, Maybe<List<(Price, CurrencyAmount)>> bids, Maybe<List<(Price, CurrencyAmount)>> asks)
        {
            var noneResult = (Maybe<PriceRange>.None, Maybe<PriceRange>.None);
            try
            {
                return history.Case(
                     some: (historyData) =>
                     {
                         if (historyData.Count > 0)
                         {
                             var firstDeriv = new List<Price>();
                             var averageTradeAmount = historyData.Average((trade) => trade.Amount);
                             var start = historyData.Last().DateCompleted;
                             var end = historyData.First().DateCompleted;
                             var timeWeight = Math.Max((end - start).TotalSeconds / historyData.Count, 1);
                             var totalWeight = 0M;
                             Price weightedTotalPrice = 0M;
                             for (int i = 0; i < historyData.Count && i < 40; i++)
                             {
                                 var weight = historyData[i].Amount * (decimal)((historyData[i].DateCompleted - start).TotalSeconds / timeWeight);
                                 totalWeight += weight;
                                 weightedTotalPrice += historyData[i].Price * weight;
                             }
                             if (totalWeight > 0)
                             {
                                 var averagePrice = weightedTotalPrice / totalWeight;
                                 var variance = historyData.Take(40).Average(historyItem => (Numeric)((historyItem.Price - averagePrice) * (historyItem.Price - averagePrice)));
                                 Price stdDeviation = Math.Sqrt(variance);

                                 var averageBid = this.GetWeightedAverage(bids, 15, averageTradeAmount * 5);

                                 var averageAsk = this.GetWeightedAverage(asks, 15, averageTradeAmount * 5);

                                 if (averageBid.HasValue() && averageAsk.HasValue())
                                 {
                                     var buyMedian = .10 * averagePrice + .90 * (.10 * averageBid.Value() + .90 * averageAsk.Value());
                                     var sellMedian = .10 * averagePrice + .90 * (.10 * averageAsk.Value() + .90 * averageBid.Value());
                                     return (Maybe.Some(new PriceRange(sellMedian, stdDeviation)),
                                         Maybe.Some(new PriceRange(buyMedian, stdDeviation)));
                                 }
                             }
                         }

                         return (Maybe<PriceRange>.None, Maybe<PriceRange>.None);
                     },
                     none: () => noneResult);
            }
            catch
            {
            }
            return noneResult;
        }

        private Maybe<Price> GetWeightedAverage(Maybe<List<(Price price, CurrencyAmount amount)>> values, int depth = 50, decimal weightDepth = 0)
        {
            return values.Case(
                some: (valuesData) =>
                {
                    var weight = 0M;
                    Price weightedTotal = 0M;
                    for (int i = 0; i < depth && i < valuesData.Count && (weightDepth != 0 && weight < weightDepth); i++)
                    {
                        weight += valuesData[i].amount;
                        weightedTotal += valuesData[i].price * valuesData[i].amount;
                    }
                    return weight != 0 ? Maybe<Price>.Some(weightedTotal / weight) : Maybe<Price>.None;
                },
                none: () => Maybe<Price>.None);
        }
        
        protected async ValueTask<T> GetValue<T>(string key, Func<ValueTask<T>> fallback, int secondsTilExpiration = 10, bool ignoreCachedValue = false)
        {
            try
            {
                AutoResetEvent lockObject = this.GetLockObject(key);
                T result;
                if (!ignoreCachedValue && this.TryGetValueCommon(key, out T cachedResult))
                {
                    result = cachedResult;
                }
                else
                {
                    result = await fallback();
                    UpdateCache(key, result, secondsTilExpiration);
                }
                lockObject.Set();
                return result;
            }
            catch
            {
                return default(T);
            }
        }
        private AutoResetEvent GetLockObject(string key)
        {
            AutoResetEvent lockObject;

            lock (dataCacheLocks)
            {
                if (!dataCacheLocks.TryGetValue(key, out lockObject))
                {
                    lockObject = new AutoResetEvent(true);
                    dataCacheLocks.Add(key, lockObject);
                }
            }
            return lockObject;
        }
        private void UpdateCache<T>(string key, T value, int secondsTilExpiration)
        {
            if (value != null)
            {
                this.dataCache.Add(new CacheItem(key, value), new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow + new TimeSpan(0, 0, secondsTilExpiration)
                });
            }
        }

        private bool TryGetValueCommon<T>(string key, out T result)
        {
            if (this.dataCache.Contains(key))
            {
                result = (T)this.dataCache[key];
                return true;
            }
            result = default(T);
            return false;          
        }
    }
}
