using Core.Functional;
using Core.Functional.TypedValues;
using CurrencyCore.Coin;
using System;

namespace ExchangesCore
{
    public class Price : TypedNumeric<RatioType<CurrencyUnit>, Price>
    {
        public readonly CurrencyType stockCurrencyType, currencyType;

        public Price(Numeric value, CurrencyType stockCurrencyType, CurrencyType currencyType) : base(value, new RatioType<CurrencyUnit>(stockCurrencyType, currencyType))
        {
            this.stockCurrencyType = stockCurrencyType;
            this.currencyType = currencyType;
        }

        protected Price(Numeric value, RatioType<CurrencyUnit> unit) : base(value, unit)
        {
        }

        public Price(TypedNumeric<RatioType<CurrencyUnit>> value) : base(value)
        {
        }       

        public static CurrencyAmount operator *(Price first, CurrencyAmount second) => second * first;

        public static CurrencyAmount operator *(CurrencyAmount first, Price second)
        {
            if(first.Currency == second.currencyType)
            {
                return new CurrencyAmount(first.Value * second.Value, second.stockCurrencyType);
            }
            else
            {
                throw new ArgumentException("The types of the price and the currency amount do not match");
            }
        }

        public override string ToString()
        {
            return string.Format("{0:F8}", (decimal)this.Value) + " " + this.Units.ToString();
        }        
    }

    public struct PriceRange
    {
        public readonly Price Mean;
        public readonly Price StdDeviation;
        public PriceRange(Price mean, Price stdDeviation)
        {
            this.Mean = mean;
            this.StdDeviation = stdDeviation;
        }

        public override string ToString()
        {
            return $"{this.Mean-this.StdDeviation} - {this.Mean + this.StdDeviation}";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
