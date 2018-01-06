using CurrencyCore.Coin;

namespace ExchangesCore
{
    public enum TradeType
    {
        Buy,
        Sell,
        Auction
    }
    public struct Trade
    {
        public readonly TradeType Type;
        public readonly CurrencyType StockCoin;
        public readonly CurrencyType CurrencyCoin;

        public Trade(TradeType type,
        CurrencyType stockCoin,
        CurrencyType currencyCoin)
        {
            this.Type = type;
            this.StockCoin = stockCoin;
            this.CurrencyCoin = currencyCoin;
        }
        public override string ToString()
        {
            return this.Type == TradeType.Buy ? $"Buy {StockCoin.ToString()} with {CurrencyCoin.ToString()}" :
                             $"Sell {StockCoin.ToString()} for {CurrencyCoin.ToString()}";
        }
    }
}
