using System;

namespace Core.Functional
{
    public class TypedNumeric
    {
        public readonly NumericType Units;
        public readonly Numeric Value;
        
        public TypedNumeric(Numeric value, NumericType units)
        {
            this.Value = value;
            this.Units = units;
        }

        public static TypedNumeric operator *(Numeric first, TypedNumeric second) => second * first;

        public static TypedNumeric operator *(TypedNumeric first, Numeric second)
        {
            return new TypedNumeric(second * first.Value, first.Units);
        }
        
        public static TypedNumeric operator /(TypedNumeric first, Numeric second)
        {
            return new TypedNumeric(first.Value / second, first.Units);
        }
        
        public static TypedNumeric operator *(TypedNumeric first, TypedNumeric second)
        {
            return new TypedNumeric(second.Value * first.Value, first.Units *second.Units);
        }
        public static TypedNumeric operator /(TypedNumeric first, TypedNumeric second)
        {
            return new TypedNumeric(second.Value / first.Value, first.Units/second.Units);
        }
        
        public static TypedNumeric operator +(TypedNumeric first, TypedNumeric second)
        {
            if (first.Units == second.Units)
            {
                return new TypedNumeric(second.Value + first.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static TypedNumeric operator -(TypedNumeric first, TypedNumeric second)
        {
            if (first.Units == second.Units)
            {
                return new TypedNumeric(second.Value + first.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static implicit operator decimal(TypedNumeric value)
        {
            return value.Value;
        }
        public static implicit operator double(TypedNumeric value)
        {
            return value.Value;
        }
        public static implicit operator float(TypedNumeric value)
        {
            return value.Value;
        }
        public static implicit operator int(TypedNumeric value)
        {
            return value.Value;
        }
        public override string ToString()
        {
            return this.Value.ToString() + " " + this.Units.ToString();
        }

    }
}
