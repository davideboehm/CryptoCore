using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyCore.Coin
{
    public abstract class CryptoCurrency : Currency
    {     
        public abstract byte GetAddressSignifier();

        public abstract byte GetWifCompressedSignifier();

        public abstract byte GetWifUncompressedSignifier();
        
        private static readonly Dictionary<CurrencyType, CryptoCurrency> CoinTypeToCoinDictionary;
        private static readonly Dictionary<string, CryptoCurrency> AbbreviationToCoinDictionary;

        static CryptoCurrency()
        {
            var assembly = typeof(CryptoCurrency).Assembly;
            var CoinClassList =
                (from type in assembly.GetTypes()
                 where type.IsSubclassOf(typeof(CryptoCurrency)) && !type.IsAbstract
                 select type).ToList();
            
            CoinTypeToCoinDictionary = new Dictionary<CurrencyType, CryptoCurrency>();
            AbbreviationToCoinDictionary = new Dictionary<string, CryptoCurrency>();
            foreach (var coin in CoinClassList.Select(coinType => (CryptoCurrency)Activator.CreateInstance(coinType)))
            {
                CoinTypeToCoinDictionary.Add(coin.GetCoinType(), coin);
                foreach (
                    var abbreviation in coin.GetAbbreviations().Select(abbreviation => abbreviation.Trim().ToLower()))
                {
                    AbbreviationToCoinDictionary.Add(abbreviation, coin);
                }
            }
        }
        
        public static CryptoCurrency GetCryptoCurrency(string abbreviation)
        {
            abbreviation = abbreviation.Trim().ToLower();
            return CryptoCurrency.AbbreviationToCoinDictionary.ContainsKey(abbreviation)
                ? CryptoCurrency.AbbreviationToCoinDictionary[abbreviation]
                : null;
        }

        public static CryptoCurrency GetCryptoCurrency(CurrencyType coinType)
        {
            return CryptoCurrency.CoinTypeToCoinDictionary.ContainsKey(coinType)
                ? CryptoCurrency.CoinTypeToCoinDictionary[coinType]
                : null;
        }
    }
}
