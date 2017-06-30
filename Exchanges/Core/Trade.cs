using CurrencyCore.Coin;

namespace ExchangesCore
{
    public enum TradeType
    {
        Buy,
        Sell,
    }
    public struct Trade
    {
        public readonly TradeType Type;
        public readonly CoinType StockCoin;
        public readonly CoinType CurrencyCoin;

        public Trade(TradeType type,
        CoinType stockCoin,
        CoinType currencyCoin)
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
