using Core.Functional;
using CurrencyCore.Address;
using CurrencyCore.Coin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangesCore
{
    public interface IExchange
    {
        ValueTask<bool> ExecuteTrade(Trade trade, Price price, CurrencyAmount amount);
        ValueTask<Maybe<CurrencyAmount>> GetBalance(CurrencyType coinType);
        ValueTask<Maybe<Dictionary<CurrencyType, CurrencyAmount>>> GetBalances(bool ignoreCache = false);
        ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetBuyMarkets();
        ValueTask<(Maybe<Fee> maker, Maybe<Fee> taker)> GetCurrentTradeFees();
        ValueTask<(Maybe<PriceRange> sellPrice, Maybe<PriceRange> buyPrice)> GetEstimatedExchangeRates(CurrencyType stockCoin, CurrencyType currencyCoin);
        ValueTask<Maybe<List<LoanOffer>>> GetLoanOrderBook(CurrencyType currencyType, int depth = 50);
        ValueTask<(Dictionary<CurrencyType, ICollection<CurrencyType>> buyMarkets, Dictionary<CurrencyType, ICollection<CurrencyType>> sellMarkets)> GetMarkets();
        ValueTask<(Maybe<List<(Price, CurrencyAmount)>> asks, Maybe<List<(Price, CurrencyAmount)>> bids)> GetOrderBook(CurrencyType stockType, CurrencyType currencyType, int depth = 50);
        ValueTask<Maybe<Dictionary<string, (List<(Price, CurrencyAmount)> asks, List<(Price, CurrencyAmount)> bids)>>> GetOrderBooks(int depth = 50);
        ValueTask<Maybe<Dictionary<CurrencyType, ICollection<CurrencyType>>>> GetSellMarkets();
        ValueTask<Maybe<List<CompletedTrade>>> GetTradeHistory(CurrencyType stockType, CurrencyType currencyType, int depth = 50);
        ValueTask<string> Withdraw(PublicAddress address, CurrencyAmount amount);
    }
}
