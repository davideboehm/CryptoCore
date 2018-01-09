using Core.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Coin
{
    public class CurrencyUnit : NumericType<CurrencyType>
    {
        public new static readonly CurrencyUnit Unit = new CurrencyUnit();
        public CurrencyUnit(CurrencyType value) : base(value)
        {
        }
        private CurrencyUnit() : base()
        {
        }
        public static implicit operator CurrencyUnit(CurrencyType type)
        {
            return new CurrencyUnit(type);
        }
    }

    public enum CurrencyType
    {
        None,
        Bitcoin,
        BitcoinCash,
        Bytecoin,
        Dash,
        DigiByte,
        Ethereum,
        EthereumClassic,
        Factom,
        Litecoin,
        Monero,
        NXT,
        Ripple,
        STEEM,
        Stellar,
        USD,
        USDTether,
        Vericoin,
        Verium,
        ZCash
    }

    public abstract class Currency
    {
        private static readonly Dictionary<string, CurrencyType> AbbreviationToCoinTypeDictionary;
        private static readonly Dictionary<CurrencyType, Currency> CoinTypeToCoinDictionary;
        private static readonly Dictionary<string, Currency> AbbreviationToCoinDictionary;

        static Currency()
        {
            var assembly = typeof(Currency).Assembly;
            var CoinClassList =
                (from type in assembly.GetTypes()
                 where type.IsSubclassOf(typeof(Currency)) && !type.IsAbstract
                 select type).ToList();

            AbbreviationToCoinTypeDictionary = new Dictionary<string, CurrencyType>();
            CoinTypeToCoinDictionary = new Dictionary<CurrencyType, Currency>();
            AbbreviationToCoinDictionary = new Dictionary<string, Currency>();
            foreach (var coin in CoinClassList.Select(coinType => (Currency)Activator.CreateInstance(coinType)))
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
        public CurrencyType GetCoinType()
        {
            return (CurrencyType)Enum.Parse(typeof(CurrencyType), this.GetCoinName());
        }

        private string coinNameCache;
        public string GetCoinName()
        {
            return this.coinNameCache ?? (this.coinNameCache = this.GetType().Name);
        }

        public static string GetDefaultAbbreviation(CurrencyType coinType)
        {
            return Currency.GetCurrency(coinType).GetAbbreviations()[0];
        }

        public static Currency GetCurrency(string abbreviation)
        {
            abbreviation = abbreviation.Trim().ToLower();
            return Currency.AbbreviationToCoinDictionary.ContainsKey(abbreviation)
                ? Currency.AbbreviationToCoinDictionary[abbreviation]
                : null;
        }
        
        public static CurrencyType GetCurrencyType(string abbreviation)
        {
            abbreviation = abbreviation.Trim().ToLower();
            return AbbreviationToCoinTypeDictionary.TryGetValue(abbreviation, out CurrencyType result) ? result : CurrencyType.None;
        }

        public static Currency GetCurrency(CurrencyType coinType)
        {
            return CoinTypeToCoinDictionary.ContainsKey(coinType)
                ? CoinTypeToCoinDictionary[coinType]
                : null;
        }
    }
}
