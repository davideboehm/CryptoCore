namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    public class Bitcoin : CoinInfo
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "BTC" };
        }
        public override byte GetWifCompressedSignifier()
        {
            return 128;
        }
        public override byte GetWifUncompressedSignifier()
        {
            return 0x80;
        }

        public override byte GetAddressSignifier()
        {
            return 0;
        }
    }    
}
