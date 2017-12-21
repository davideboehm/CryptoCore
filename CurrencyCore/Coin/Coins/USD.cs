namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    internal class USD : Currency
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "USD" };
        }
    }
}
