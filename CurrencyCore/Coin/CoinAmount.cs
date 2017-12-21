namespace CurrencyCore.Coin
{
    using Core.Functional;
    using System;
    /// <summary>
    /// Used to store coin quantities. 
    /// ** WARNING** Only holds 8 decimal places since most coins have a limit of 8
    /// </summary>
    public struct CurrencyAmount
    {
        public static CurrencyAmount Zero = (CurrencyAmount)decimal.Zero;
        private decimal value;

        public static Maybe<CurrencyAmount> operator *(CurrencyAmount value1, Maybe<CurrencyAmount> value2)
        {
            return value2.Case(
                some: (someData) => Maybe.Some(value1 * someData),
                none: () => Maybe<CurrencyAmount>.None);
        }
        public static Maybe<CurrencyAmount> operator *(Maybe<CurrencyAmount> value1, CurrencyAmount value2)
        {
            return value1.Case(
                some: (someData) => Maybe.Some(value2 * someData),
                none: () => Maybe<CurrencyAmount>.None);
        }

        public static implicit operator CurrencyAmount(decimal amount)
        {
            return new CurrencyAmount { value = Math.Round(amount, 8) };
        }

        public static implicit operator decimal(CurrencyAmount amount)
        {
            return amount.value;
        }

        public static CurrencyAmount operator +(CurrencyAmount c1, CurrencyAmount c2)
        {
            return (CurrencyAmount)(c1.value + c2.value);
        }

        public static CurrencyAmount operator -(CurrencyAmount c1, CurrencyAmount c2)
        {
            return (CurrencyAmount)(c1.value - c2.value);
        }

        public static CurrencyAmount operator *(CurrencyAmount c1, CurrencyAmount c2)
        {
            return (CurrencyAmount)(c1.value * c2.value);
        }

        public static CurrencyAmount operator /(CurrencyAmount c1, CurrencyAmount c2)
        {
            return (CurrencyAmount)(c1.value / c2.value);
        }

        public override string ToString()
        {
            return string.Format("{0:F8}", this.value);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
