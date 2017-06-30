﻿namespace CurrencyCore.Coin.Coins
{
    using System;
    using System.Collections.Generic;

    internal class DigiByte : CoinInfo
    {
        public override List<string> GetAbbreviations()
        {
            return new List<string> { "DGB" };
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

