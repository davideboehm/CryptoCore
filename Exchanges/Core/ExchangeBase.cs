using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyCore.Address;
using CurrencyCore.Coin;
using System.Runtime.Caching;
using System.Threading;

namespace ExchangesCore
{
    public abstract class ExchangeBase : IExchange
    {
        private MemoryCache dataCache;
        private Dictionary<string, AutoResetEvent> dataCacheLocks = new Dictionary<string, AutoResetEvent>();

        public abstract ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount);
        public abstract ValueTask<CoinAmount?> GetBalance(CoinType coinType);
        public abstract ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances(bool ignoreCache = false);
        public abstract ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets();
        public abstract ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees();
        public abstract ValueTask<List<LoanOffer>> GetLoanOrderBook(CoinType currencyType, int depth = 50);
        public abstract ValueTask<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)> GetMarkets();
        public abstract ValueTask<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)> GetOrderBook(CoinType stockType, CoinType currencyType, int depth = 50);
        public abstract ValueTask<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>> GetOrderBooks(int depth = 50);
        public abstract ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetSellMarkets();
        public abstract ValueTask<List<CompletedTrade>> GetTradeHistory(CoinType stockType, CoinType currencyType, int depth = 50);
        public abstract ValueTask<string> Withdraw(PublicAddress address, CoinAmount amount);

        protected ExchangeBase(string className)
        {
            this.dataCache = new MemoryCache(className + Thread.CurrentThread.ManagedThreadId);
        }

        public virtual async ValueTask<(PriceRange? sellPrice, PriceRange? buyPrice)> GetEstimatedExchangeRates(CoinType stockCoin, CoinType currencyCoin)
        {
            var history = await this.GetTradeHistory(stockCoin, currencyCoin);

            var (asks, bids) = await this.GetOrderBook(stockCoin, currencyCoin);
            return this.GetEstimatedExchangeRate(history, bids, asks);
        }

        protected (PriceRange?, PriceRange?) GetEstimatedExchangeRate(List<CompletedTrade> history, List<(Price, CoinAmount)> bids, List<(Price, CoinAmount)>  asks)
        {
            if (history == null || bids == null || asks == null)
            {
                return (null,null);
            }

            var firstDeriv = new List<Price>();
            var averageTradeAmount = history.Average((trade) => trade.Amount);
            var start = history.Last().DateCompleted;
            var end = history.First().DateCompleted;
            var timeWeight = (end - start).TotalSeconds / history.Count;
            var totalWeight = 0M;
            var weightedTotalPrice = 0M;
            for (int i = 0; i < history.Count && i < 40; i++)
            {
                var weight = history[i].Amount * (decimal) ((history[i].DateCompleted - start).TotalSeconds / timeWeight);
                totalWeight += weight;
                weightedTotalPrice += history[i].Price * weight;
            }
            var averagePrice = (double)(weightedTotalPrice / totalWeight);
            var variance = history.Take(40).Average(historyItem =>((double)(historyItem.Price - averagePrice)* (double)(historyItem.Price - averagePrice)));
            Price stdDeviation = Math.Sqrt(variance);

            var averageBid = this.GetWeightedAverage(bids, 15, averageTradeAmount * 5);

            var averageAsk = this.GetWeightedAverage(asks, 15, averageTradeAmount * 5);

            var buyMedian = .10 * averagePrice + .90 * (.10 * averageBid + .90 * averageAsk);
            var sellMedian = .10 * averagePrice + .90 * (.10 * averageAsk + .90 * averageBid);
            return (new PriceRange(sellMedian, stdDeviation),
                    new PriceRange(buyMedian, stdDeviation));
        }

        private Price GetWeightedAverage(List<(Price price, CoinAmount amount)> values, int depth = 50, decimal weightDepth = 0)
        {
            var weight = 0M;
            var weightedTotal = 0M;
            for (int i = 0; i < depth && i < values.Count && (weightDepth != 0 && weight < weightDepth); i++)
            {
                weight += values[i].amount;
                weightedTotal += values[i].price * values[i].amount;
            }

            return weightedTotal / weight;
        }
        
        protected async ValueTask<T> GetValue<T>(string key, Func<ValueTask<T>> fallback, int secondsTilExpiration = 10, bool ignoreCachedValue = false)
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
