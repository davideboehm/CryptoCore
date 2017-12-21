
namespace ExchangesCore
{
    using CurrencyCore.Coin;
    using System;

    public struct Fee
    {
        public readonly decimal Percent;
        public readonly CurrencyAmount Flat;
        
        public Fee(Decimal percentageFee, CurrencyAmount flatFee)
        {
            this.Percent = percentageFee;
            this.Flat = flatFee;
        }
    }
}
