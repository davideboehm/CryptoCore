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
        public readonly Price Min;
        public readonly Price Max;
        public PriceRange(Price min, Price max)
        {
            this.Min = Math.Min((decimal)min, max);
            this.Max = Math.Max((decimal)min, max);
        }

        public override string ToString()
        {
            return $"{this.Min} - {this.Max}";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
