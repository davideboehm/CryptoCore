using Core.Functional.TypedValues;
using System;
using System.Reflection;

namespace Core.Functional
{
    public class TypedNumeric
    {
        public readonly NumericType Units;
        public readonly Numeric Value;

        protected TypedNumeric()
        {
        }

        public TypedNumeric(Numeric value, NumericType units)
        {
            this.Value = value;
            this.Units = units;
        }

        public static explicit operator Numeric(TypedNumeric value)
        {
            return value.Value;
        }

        public static TypedNumeric operator /(TypedNumeric first, TypedNumeric second)
        {
            return new TypedNumeric(second.Value / first.Value, first.Units / second.Units);
        }

        public static TypedNumeric operator *(TypedNumeric first, TypedNumeric second)
        {
            return new TypedNumeric(second.Value * first.Value, first.Units * second.Units);
        }

        public static TypedNumeric operator *(TypedNumeric first, Numeric second)
        {
            return new TypedNumeric(first.Value * second, first.Units);
        }
        public static TypedNumeric operator *(Numeric first, TypedNumeric second) => second * first;
    }

    public class TypedNumeric<T>: TypedNumeric where T : NumericType
    {
        public new readonly T Units;
        public TypedNumeric(Numeric value, T units): base(value,units)
        {
            this.Units = units;
        }

        public static TypedNumeric<T> operator *(Numeric first, TypedNumeric<T> second) => second * first;

        public static TypedNumeric<T> operator *(TypedNumeric<T> first, Numeric second)
        {
            return new TypedNumeric<T>(second * first.Value, first.Units);
        }
                
        public static TypedNumeric<RatioType<T>> operator /(TypedNumeric<T> first, TypedNumeric<T> second)
        {
            return new TypedNumeric<RatioType<T>>(first.Value / second.Value, new RatioType<T>(first.Units, second.Units));
        }

        public static TypedNumeric<T> operator /(TypedNumeric<T> first, Numeric second)
        {
            return new TypedNumeric<T>(first.Value / second, first.Units);
        }

        public static TypedNumeric<T> operator +(TypedNumeric<T> first, TypedNumeric<T> second)
        {
            if (first.Units == second.Units || second.Units == NumericType.Unit)
            {
                return new TypedNumeric<T>(second.Value + first.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static TypedNumeric<T> operator -(TypedNumeric<T> first, TypedNumeric<T> second)
        {
            if (first.Units == second.Units || second.Units == NumericType.Unit)
            {
                return new TypedNumeric<T>(second.Value + first.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static implicit operator decimal(TypedNumeric<T> value)
        {
            return value.Value;
        }

        public static implicit operator double(TypedNumeric<T> value)
        {
            return value.Value;
        }

        public static implicit operator float(TypedNumeric<T> value)
        {
            return value.Value;
        }

        public static implicit operator int(TypedNumeric<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString() + " " + this.Units.ToString();
        }

    }

    public class TypedNumeric<T, U> : TypedNumeric<T> where T : NumericType where U : TypedNumeric<T, U>
    {
        public TypedNumeric(Numeric value, T units) : base(value, units)
        {
        }

        public TypedNumeric(TypedNumeric<T> value) : base(value.Value, value.Units)
        {
        }
        
        public static U operator /(TypedNumeric<T, U> first, Numeric second)
        {
            return TypedNumeric<T, U>.Generate(first.Value / second, first.Units);
        }

        public static U operator *(Numeric first,  TypedNumeric<T, U> second) => second * first;

        public static U operator *(TypedNumeric<T, U> first, Numeric second)
        {
            return TypedNumeric<T, U>.Generate(first.Value * second, first.Units);
        }

        public static U operator +(TypedNumeric<T, U> first, TypedNumeric<T, U> second)
        {
            if (first.Units == second.Units)
            {
                return TypedNumeric<T, U>.Generate(first.Value + second.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static U operator -(TypedNumeric<T, U> first, TypedNumeric<T, U> second)
        {
            if (first.Units == second.Units)
            {
                return TypedNumeric<T, U>.Generate(first.Value - second.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        public static U operator -(TypedNumeric<T, U> first, TypedNumeric second)
        {
            if (second.Units == NumericType.Unit || first.Units == second.Units)
            {
                return TypedNumeric<T, U>.Generate(first.Value - second.Value, first.Units);
            }

            throw new ArgumentException("The units of these two values do not match");
        }

        private static U Generate(Numeric value, T units)
        {
            return (U)Activator.CreateInstance(typeof(U),
                                                 BindingFlags.CreateInstance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance |
                                                 BindingFlags.OptionalParamBinding,
                                                 null, new Object[] { value, units }, null);
        }
    }
}
