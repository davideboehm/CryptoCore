using CurrencyCore.Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangesCore
{
    public struct LoanOffer
    {
        public LoanOffer(Decimal rate, CurrencyAmount quantity, TimeSpan minLength, TimeSpan maxLength)
        {
            this.Rate = rate;
            this.Quantity = quantity;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
        }

        public Decimal Rate { get; private set;}
        public CurrencyAmount Quantity { get; private set; }

        public TimeSpan MinLength { get; private set; }
        public TimeSpan MaxLength { get; private set; }
    }
}
