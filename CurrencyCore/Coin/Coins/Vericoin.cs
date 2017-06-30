namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    internal class Vericoin : CoinInfo
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "VRC" };
        }
        public override byte GetAddressSignifier()
        {
            return 70;
        }
        public override byte GetWifCompressedSignifier()
        {
            return 198;
        }
        public override byte GetWifUncompressedSignifier()
        {
            throw new NotImplementedException();
        }
    }
}

