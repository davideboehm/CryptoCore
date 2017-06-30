namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    public class USDTether : CoinInfo
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "USDT" };
        }
        public override byte GetAddressSignifier()
        {
            throw new NotImplementedException();
        }
        public override byte GetWifCompressedSignifier()
        {
            throw new NotImplementedException();
        }
        public override byte GetWifUncompressedSignifier()
        {
            throw new NotImplementedException();
        }
    }
}
