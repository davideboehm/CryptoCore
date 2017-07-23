using CurrencyCore.Address;
using CurrencyCore.Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangesCore
{
    public interface IExchange
    {
        ValueTask<bool> ExecuteTrade(Trade trade, Price price, CoinAmount amount);
        ValueTask<CoinAmount?> GetBalance(CoinType coinType);
        ValueTask<Dictionary<CoinType, CoinAmount>> GetBalances(bool ignoreCache = false);
        ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetBuyMarkets();
        ValueTask<(Fee? maker, Fee? taker)> GetCurrentTradeFees();
        ValueTask<(PriceRange? sellPrice, PriceRange? buyPrice)> GetEstimatedExchangeRates(CoinType stockCoin, CoinType currencyCoin);
        ValueTask<List<LoanOffer>> GetLoanOrderBook(CoinType currencyType, int depth = 50);
        ValueTask<(Dictionary<CoinType, ICollection<CoinType>> buyMarkets, Dictionary<CoinType, ICollection<CoinType>> sellMarkets)> GetMarkets();
        ValueTask<(List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)> GetOrderBook(CoinType stockType, CoinType currencyType, int depth = 50);
        ValueTask<Dictionary<string, (List<(Price, CoinAmount)> asks, List<(Price, CoinAmount)> bids)>> GetOrderBooks(int depth = 50);
        ValueTask<Dictionary<CoinType, ICollection<CoinType>>> GetSellMarkets();
        ValueTask<List<CompletedTrade>> GetTradeHistory(CoinType stockType, CoinType currencyType, int depth = 50);
        ValueTask<string> Withdraw(PublicAddress address, CoinAmount amount);
    }
}
