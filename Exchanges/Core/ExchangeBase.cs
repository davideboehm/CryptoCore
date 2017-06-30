using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public abstract ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount, bool postOnly = false);
        public abstract ValueTask<CoinAmount?> GetBalance(CoinType coinType);
        public abstract ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances();
        public abstract ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets();
        public abstract ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees();
        public abstract ValueTask<Price?> GetEstimatedBuyExchangeRate(CoinType stockCoin, CoinType currencyCoin);
        public abstract ValueTask<Price?> GetEstimatedSellExchangeRate(CoinType stockCoin, CoinType currencyCoin);
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

        protected PriceRange? GetEstimatedExchangeRate(List<CompletedTrade> history, List<(Price, CoinAmount)> offers)
        {
            if (history == null || offers == null)
            {
                return null;
            }

            var averagedValues = new List<Price>();
            var firstDeriv = new List<Price>();
            var averageTradeAmount = history.Average((trade) => trade.Amount);

            var retainedTotal = 0M;
            var retainedWeight = 0M;
            for (int i = 0; i < history.Count - 1 && averagedValues.Count < 10; i++)
            {
                var currentTotal = retainedTotal;
                var currentWeightedTotal = retainedWeight;

                while (currentTotal < averageTradeAmount && i < history.Count)
                {
                    if (currentTotal + history[i].Amount > averageTradeAmount)
                    {
                        retainedTotal = history[i].Amount + currentTotal - averageTradeAmount;
                        retainedWeight = retainedTotal * history[i].Price;

                        if (retainedTotal > averageTradeAmount)
                        {
                            averagedValues.Add(retainedWeight / retainedTotal);
                            retainedTotal = 0M;
                            retainedWeight = 0M;
                        }

                        currentWeightedTotal += history[i].Price * (history[i].Amount - retainedTotal);
                        currentTotal += (history[i].Amount - retainedTotal);
                    }
                    else
                    {
                        retainedTotal = 0M;
                        retainedWeight = 0M;

                        currentWeightedTotal += history[i].Price * history[i].Amount;
                        currentTotal += history[i].Amount;
                    }
                    i++;
                }
                averagedValues.Add(currentWeightedTotal / currentTotal);
            }
            for (int i = 0; i < 10 && i < averagedValues.Count - 1; i++)
            {
                firstDeriv.Add(averagedValues[i] - averagedValues[i + 1]);
            }

            var averageBid = this.GetWeightedAverage(offers, 15, averageTradeAmount * 5);
            var averagePrice = averagedValues.Average(price => (double)price);
            var variation = Math.Sqrt(averagedValues.Average(price => (averagePrice - (double)price) * (averagePrice - (double)price)));
            var median = .5 * averagePrice + .5 * (double)averageBid;
            return new PriceRange((Price)(median - variation), (Price)(median + variation));

        }
        private Price GetWeightedAverage(List<(Price price, CoinAmount amount)> values, int depth = 50, decimal weightDepth = 0)
        {
            var total = 0M;
            var weightedTotal = 0M;
            for (int i = 0; i < depth && i < values.Count && (weightDepth != 0 && total < weightDepth); i++)
            {
                total += values[i].amount;
                weightedTotal += values[i].price * values[i].amount;
            }

            return weightedTotal / total;
        }
        protected async ValueTask<T> GetValueFromCache<T>(string key, Func<ValueTask<T>> fallback, int secondsTilExpiration = 10)
        {
            T result;
            AutoResetEvent lockObject;

            lock (dataCacheLocks)
            {
                if (!dataCacheLocks.TryGetValue(key, out lockObject))
                {
                    lockObject = new AutoResetEvent(true);
                    dataCacheLocks.Add(key, lockObject);
                }
            }
            lockObject.WaitOne();

            if (this.TryGetItemFromCache(key, out T cachedResult))
            {
                result = cachedResult;
            }
            else
            {
                result = await fallback(); if (result != null)
                {
                    this.dataCache.Add(new CacheItem(key, result), new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow + new TimeSpan(0, 0, secondsTilExpiration)
                    });
                }
            }
            lockObject.Set();
            return result;
        }
        protected T GetValueFromCache<T>(string key, Func<T> fallback, int secondsTilExpiration = 10)
        {
            T result;
            AutoResetEvent lockObject;
            lock (dataCacheLocks)
            {
                if (!dataCacheLocks.TryGetValue(key, out lockObject))
                {
                    lockObject = new AutoResetEvent(true);
                    dataCacheLocks.Add(key, lockObject);
                }
            }

            lockObject.WaitOne();

            if (this.TryGetItemFromCache(key, out T cachedResult))
            {
                result = cachedResult;
            }
            else
            {
                result = fallback();
                if (result != null)
                {
                    this.dataCache.Add(new CacheItem(key, result), new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow + new TimeSpan(0, 0, secondsTilExpiration)
                    });
                }
            }
            lockObject.Set();
            return result;
        }

        protected bool TryGetItemFromCache<T>(string key, out T item)
        {
            if (this.dataCache.Contains(key))
            {
                item = (T)this.dataCache[key];
                return true;
            }
            item = default(T);
            return false;
        }
    }
}
