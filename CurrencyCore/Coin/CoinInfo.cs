namespace CurrencyCore.Coin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum CoinType
    {
        None,
        Bitcoin,
        Dash,
        DigiByte,
        Ethereum,
        EthereumClassic,
        Factom,
        Litecoin,
        Monero,
        Ripple,
        Stellar,
        USDTether,
        Vericoin,
        Verium,
        ZCash
    }
    public abstract class CoinInfo
    {
        private static readonly List<Type> CoinClassList;
        private static readonly Dictionary<string, CoinType> AbbreviationToCoinTypeDictionary;
        private static readonly Dictionary<CoinType, CoinInfo> CoinTypeToCoinDictionary;
        private static readonly Dictionary<string, CoinInfo> AbbreviationToCoinDictionary;

        static CoinInfo()
        {
            var assembly = typeof(CoinInfo).Assembly;               
            CoinClassList =
                (from type in assembly.GetTypes()
                 where type.IsSubclassOf(typeof(CoinInfo))
                 select type).ToList();

            AbbreviationToCoinTypeDictionary = new Dictionary<string, CoinType>();
            CoinTypeToCoinDictionary = new Dictionary<CoinType, CoinInfo>();
            AbbreviationToCoinDictionary = new Dictionary<string, CoinInfo>();
            foreach (var coin in CoinClassList.Select(coinType => (CoinInfo)Activator.CreateInstance(coinType)))
            {
                CoinTypeToCoinDictionary.Add(coin.GetCoinType(), coin);
                foreach (
                    var abbreviation in coin.GetAbbreviations().Select(abbreviation => abbreviation.Trim().ToLower()))
                {
                    AbbreviationToCoinDictionary.Add(abbreviation, coin);
                    AbbreviationToCoinTypeDictionary.Add(abbreviation, coin.GetCoinType());
                }
            }
        }

        public abstract List<string> GetAbbreviations();
        public abstract byte GetAddressSignifier();

        public abstract byte GetWifCompressedSignifier();

        public abstract byte GetWifUncompressedSignifier();
        public CoinType GetCoinType()
        {
            return (CoinType)Enum.Parse(typeof(CoinType), this.GetCoinName());
        }

        private string coinNameCache;
        public string GetCoinName()
        {
            return this.coinNameCache ?? (this.coinNameCache = this.GetType().Name);
        }

        public static CoinType GetCoinType(string abbreviation)
        {
            abbreviation = abbreviation.Trim().ToLower();
            return AbbreviationToCoinTypeDictionary.TryGetValue(abbreviation, out CoinType result) ? result : CoinType.None;
        }

        public static CoinInfo GetCoinInfo(CoinType coinType)
        {
            return CoinTypeToCoinDictionary.ContainsKey(coinType)
                ? CoinTypeToCoinDictionary[coinType]
                : null;
        }
        public static string GetDefaultAbbreviation(CoinType coinType)
        {
            return GetCoinInfo(coinType).GetAbbreviations()[0];
        }

        public static CoinInfo GetCoinInfo(string abbreviation)
        {
            abbreviation = abbreviation.Trim().ToLower();
            return AbbreviationToCoinDictionary.ContainsKey(abbreviation)
                ? AbbreviationToCoinDictionary[abbreviation]
                : null;
        }
    }
}
