using Core.Functional;
using Core.Functional.TypedValues;
using CurrencyCore.Coin;
using System;

namespace ExchangesCore
{
    public class Price2 : TypedNumeric
    {
        public readonly CurrencyType stockCurrencyType, currencyType;
        public Price2(Numeric value, CurrencyType stockCurrencyType, CurrencyType currencyType) : base(value, new RatioType<CurrencyType>(stockCurrencyType, currencyType))
        {
            this.stockCurrencyType = stockCurrencyType;
            this.currencyType = currencyType;
        }
        public static CurrencyAmount2 operator *(CurrencyAmount2 first, Price2 second)
        {
            if(first.currency == second.currencyType)
            {
                return new CurrencyAmount2(first.Value * second.Value, second.stockCurrencyType);
            }
            else
            {
                throw new ArgumentException("The types of the price and the currency amount do not match");
            }
        }

        public static CurrencyAmount2 operator *(Price2 first, CurrencyAmount2 second) => second * first;

        public override string ToString()
        {
            return string.Format("{0:F8}", (decimal)this.Value) + " " + this.Units.ToString();
        }
    }

    public struct Price
    {
        public static Price Zero = Numeric.Zero;
        private Numeric value;

        public static implicit operator Maybe<Price>(Price value)
        {
            return Maybe.Some(value);
        }

        public static implicit operator Maybe<Numeric>(Price value)
        {
            return Maybe<Numeric>.Some(value.value);
        }

        public static implicit operator MaybeNumeric(Price value)
        {
            return MaybeNumeric.Some(value.value);
        }
        
        public static implicit operator Price(decimal amount)
        {
            return new Price { value = Math.Round(amount, 8) };
        }
                
        public static implicit operator Price(double amount)
        {
            return new Price { value = Math.Round(amount, 8) };
        }

        public static implicit operator Price(Numeric amount)
        {
            return new Price { value = Math.Round((decimal)amount, 8) };
        }

        public static implicit operator Numeric(Price amount)
        {
            return amount.value;
        }
        
        public override bool Equals(object obj)
        {
           return obj is Price && this == (Price)obj;
        }
           
        public static bool operator ==(Price c1, Price c2)
        {
            return (c1.value == c2.value);
        }

        public static bool operator !=(Price c1, Price c2)
        {
            return (c1.value != c2.value);
        }

        public static bool operator >(Price c1, Price c2)
        {
            return (c1.value > c2.value);
        }

        public static bool operator <(Price c1, Price c2)
        {
            return (c1.value < c2.value);
        }

        public static Price operator +(Price c1, Price c2)
        {
            return (c1.value + c2.value);
        }
        
        public static Price operator -(Price c1, Price c2)
        {
            return (c1.value - c2.value);
        }

        public static Price operator *(Price c1, Price c2)
        {
            return (c1.value * c2.value);
        }

        public static Price operator *(CurrencyAmount c1, Price c2)
        {
            return (Numeric) c1 * c2.value;
        }

        public static Price operator *(Price c1, CurrencyAmount c2)
        {
            return c2 * c1;
        }

        public static Price operator /(Price c1, Price c2)
        {
            return (c1.value / c2.value);
        }

        public override string ToString()
        {
            return string.Format("{0:F8}", this.value);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
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
