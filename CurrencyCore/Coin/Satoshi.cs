using Core.Functional;

namespace CurrencyCore.Coin
{
    /// <summary>
    /// Represents a quantity of cryptoCoin.  One Satoshi is equal to 10^(-8) of a coin
    /// </summary>
    public class Satoshi : TypedNumeric<CurrencyUnit, Satoshi>
    {
        private const decimal TenToTheNegativeEighth = 0.00000001M;
        private const decimal TenToTheEighth = 100000000M;

        public static readonly Satoshi Zero = new Satoshi(0, CurrencyUnit.Unit);

        public readonly CurrencyType Currency;
        public Satoshi(Numeric value, CurrencyType currency) : base(value, new CurrencyUnit(currency))
        {
            this.Currency = currency;
        }

        protected Satoshi(Numeric value, CurrencyUnit unit) : base(value, unit)
        {
        }

        public static explicit operator Satoshi(CurrencyAmount amount)
        {
            return new Satoshi ((amount * TenToTheEighth).Value, amount.Currency);
        }

        public static explicit operator CurrencyAmount(Satoshi amount)
        {
            return new CurrencyAmount((amount * TenToTheNegativeEighth).Value, amount.Currency);
        }

        public override string ToString()
        {
            return this.Value.ToString() + " Satoshi (" + this.Currency.ToString()+")";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
