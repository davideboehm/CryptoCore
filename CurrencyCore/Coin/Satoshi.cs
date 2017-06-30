namespace CurrencyCore.Coin
{
    /// <summary>
    /// Represents a quantity of cryptoCoin.  One Satoshi is equal to 10^(-8) of a coin
    /// </summary>
    public struct Satoshi
    {
        private const decimal TenToTheNegativeEighth = 0.00000001M;
        private const decimal TenToTheEighth = 100000000M;
        private long value;

        public static explicit operator Satoshi(CoinAmount amount)
        {
            return new Satoshi { value = (long)(amount * TenToTheEighth) };
        }

        public static explicit operator CoinAmount(Satoshi amount)
        {
            return (CoinAmount)(amount.value * TenToTheNegativeEighth);
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
