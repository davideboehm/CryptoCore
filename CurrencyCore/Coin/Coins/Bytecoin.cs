﻿namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    internal class Bytecoin : CryptoCurrency
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "BCN" };
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

