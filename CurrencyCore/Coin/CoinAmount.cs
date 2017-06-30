namespace CurrencyCore.Coin
{
    using System;
    /// <summary>
    /// Used to store coin quantities. 
    /// ** WARNING** Only holds 8 decimal places since most coins have a limit of 8
    /// </summary>
    public struct CoinAmount
    {
        public static CoinAmount Zero = (CoinAmount)decimal.Zero;
        private decimal value;

        public static explicit operator CoinAmount(decimal amount)
        {
            return new CoinAmount { value = Math.Round(amount, 8) };
        }

        public static implicit operator decimal(CoinAmount amount)
        {
            return amount.value;
        }

        public static CoinAmount operator +(CoinAmount c1, CoinAmount c2)
        {
            return (CoinAmount)(c1.value + c2.value);
        }

        public static CoinAmount operator -(CoinAmount c1, CoinAmount c2)
        {
            return (CoinAmount)(c1.value - c2.value);
        }

        public static CoinAmount operator *(CoinAmount c1, CoinAmount c2)
        {
            return (CoinAmount)(c1.value * c2.value);
        }

        public static CoinAmount operator /(CoinAmount c1, CoinAmount c2)
        {
            return (CoinAmount)(c1.value / c2.value);
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
