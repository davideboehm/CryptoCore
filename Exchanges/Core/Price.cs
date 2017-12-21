using Core.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangesCore
{
    public struct Price
    {
        public static Price Zero = (Price)decimal.Zero;
        private decimal value;

        public static implicit operator Maybe<Price>(Price value)
        {
            return Maybe.Some(value);
        }
        
        public static implicit operator MaybeDecimal(Price value)
        {
            return MaybeDecimal.Some(value.value);
        }

        public static implicit operator MaybeDouble(Price value)
        {
            return MaybeDouble.Some((double)value.value);
        }

        public override bool Equals(object obj)
        {
           return obj is Price && this == (Price)obj;
        }

        public static implicit operator Price(decimal amount)
        {
            return new Price { value = Math.Round(amount, 8) };
        }

        public static implicit operator Price(double amount)
        {
            return new Price { value =(decimal) Math.Round(amount, 8) };
        }

        public static implicit operator double(Price amount)
        {
            return (double) amount.value;
        }

        public static implicit operator decimal(Price amount)
        {
            return amount.value;
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
            return (Price)(c1.value + c2.value);
        }

        public static Price operator -(Price c1, Price c2)
        {
            return (Price)(c1.value - c2.value);
        }

        public static Price operator *(Price c1, Price c2)
        {
            return (Price)(c1.value * c2.value);
        }

        public static Price operator /(Price c1, Price c2)
        {
            return (Price)(c1.value / c2.value);
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
