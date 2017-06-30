
namespace ExchangesCore
{
    using CurrencyCore.Coin;
    using System;

    public struct Fee
    {
        public readonly decimal Percent;
        public readonly CoinAmount Flat;
        
        public Fee(Decimal percentageFee, CoinAmount flatFee)
        {
            this.Percent = percentageFee;
            this.Flat = flatFee;
        }
    }
}
