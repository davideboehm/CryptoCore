namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    internal class EthereumClassic : CryptoCurrency
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "ETC" };
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

