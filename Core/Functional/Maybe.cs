using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Functional
{
    public static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value) where T : class
        {
            return value != null
                ? Maybe.Some(value)
                : Maybe<T>.None;
        }

        public static Maybe<T> ToMaybe<T>(this T? nullable) where T : struct
        {
            return nullable.HasValue
                ? Maybe.Some(nullable.Value)
                : Maybe<T>.None;
        }

        public static Maybe<string> NoneIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s)
                ? Maybe<string>.None
                : Maybe.Some(s);
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> self) where T : class
        {
            return self.FirstOrDefault().ToMaybe();
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T?> self) where T : struct
        {
            return self.FirstOrDefault().ToMaybe();
        }
    }

    public interface IMaybe<T>
    {
        bool HasValue();
        T Value();
        U Case<U>(Func<T, U> some, Func<U> none);
    }

    public static class Maybe
    {
        public static Maybe<T> Some<T>(T value)
        {
            return Maybe<T>.Some(value);
        }

    }

    public struct Maybe<T> : IMaybe<T>
    {
        readonly T value;

        public static Maybe<T> None => new Maybe<T>();

        public static Maybe<T> Some(T value)
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            return new Maybe<T>(value);
        }

        private Maybe(T value)
        {
            this.value = value;
        }

        public bool HasValue() => value != null;

        public T Value()
        {
            if (!HasValue())
            {
                throw new InvalidOperationException("Maybe does not have a value");
            }

            return value;
        }

        public U Case<U>(Func<T, U> some, Func<U> none)
        {
            return this.HasValue()
              ? some(this.Value())
              : none();
        }
    }
    public struct MaybeNumeric : IMaybe<Numeric>
    {
        public static MaybeNumeric Some(Numeric value)
        {
            return new MaybeNumeric(Maybe.Some(value));
        }

        private Maybe<Numeric> underlying;
        private MaybeNumeric(Maybe<Numeric> value)
        {
            this.underlying = value;
        }

        public static MaybeNumeric operator /(MaybeNumeric value1, MaybeNumeric value2)
        {
            if (value1.HasValue() && value2.HasValue() && value2.Value() != 0)
            {
                return new MaybeNumeric(Maybe.Some(value1.Value() / value2.Value()));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric operator /(MaybeNumeric value1, Numeric value2)
        {
            if (value1.HasValue() && value2 != 0)
            {
                return new MaybeNumeric(Maybe.Some(value1.Value() / value2));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric operator /(Numeric value1, MaybeNumeric value2)
        {
            if ( value2.HasValue() && value2.Value() != 0)
            {
                return new MaybeNumeric(Maybe.Some(value1 / value2.Value()));
            }
            return MaybeNumeric.None;
        }


        public static MaybeNumeric operator +(MaybeNumeric value2, Numeric value1)
        {
            if (value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric operator *(MaybeNumeric value2, Numeric value1)
        {
            if (value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeNumeric.None;
        }
                
        public static MaybeNumeric operator +(Numeric value1, MaybeNumeric value2)
        {
            if (value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric operator *(Numeric value1, MaybeNumeric value2)
        {
            if (value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeNumeric.None;
        }
                
        public static MaybeNumeric operator +(MaybeNumeric value1, MaybeNumeric value2)
        {
            if (value1.HasValue() && value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1.Value() + value2.Value()));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric operator *(MaybeNumeric value1, MaybeNumeric value2)
        {
            if (value1.HasValue() && value2.HasValue())
            {
                return new MaybeNumeric(Maybe.Some(value1.Value() * value2.Value()));
            }
            return MaybeNumeric.None;
        }

        public static MaybeNumeric None => new MaybeNumeric(Maybe<Numeric>.None);

        public static implicit operator MaybeNumeric(Numeric current)
        {
            return new MaybeNumeric(Maybe.Some(current));
        }

        public static implicit operator MaybeNumeric(Maybe<Numeric> current)
        {
            return new MaybeNumeric(current);
        }
        
        public static implicit operator Maybe<Numeric>(MaybeNumeric current)
        {
            return current.underlying;
        }

        public U Case<U>(Func<Numeric, U> some, Func<U> none)
        {
            return underlying.Case(some, none);
        }

        public bool HasValue()
        {
            return underlying.HasValue();
        }

        public Numeric Value()
        {
            return underlying.Value();
        }
    }

    /*
    public struct MaybeDecimal : IMaybe<Decimal>
    {
        public static MaybeDecimal Some(Decimal value)
        {
            return new MaybeDecimal(Maybe.Some(value));
        }

        private Maybe<Decimal> underlying;
        private MaybeDecimal(Maybe<Decimal> value)
        {
            this.underlying = value;
        }
        public static MaybeDecimal operator /(MaybeDecimal value2, Decimal value1)
        {
            if (value2.HasValue() && value1 != 0)
            {
                return new MaybeDecimal(Maybe.Some(value2.Value() / value1));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator +(MaybeDecimal value2, Decimal value1)
        {
            if (value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator *(MaybeDecimal value2, Decimal value1)
        {
            if (value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeDecimal.None;
        }


        public static MaybeDecimal operator /(Decimal value1, MaybeDecimal value2)
        {
            if (value2.HasValue() && value2.Value() != 0)
            {
                return new MaybeDecimal(Maybe.Some(value1 / value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator +(Decimal value1, MaybeDecimal value2)
        {
            if (value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator *(Decimal value1, MaybeDecimal value2)
        {
            if (value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeDecimal.None;
        }


        public static MaybeDecimal operator /(MaybeDecimal value1, MaybeDecimal value2)
        {
            if (value1.HasValue() && value2.HasValue() && value2.Value() != 0)
            {
                return new MaybeDecimal(Maybe.Some(value1.Value() / value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator +(MaybeDecimal value1, MaybeDecimal value2)
        {
            if (value1.HasValue() && value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1.Value() + value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal operator *(MaybeDecimal value1, MaybeDecimal value2)
        {
            if (value1.HasValue() && value2.HasValue())
            {
                return new MaybeDecimal(Maybe.Some(value1.Value() * value2.Value()));
            }
            return MaybeDecimal.None;
        }

        public static MaybeDecimal None => new MaybeDecimal(Maybe<Decimal>.None);

        public static implicit operator MaybeDecimal(Decimal current)
        {
            return new MaybeDecimal(Maybe.Some(current));
        }

        public static implicit operator MaybeDecimal(Maybe<Decimal> current)
        {
            return new MaybeDecimal(current);
        }

        public static implicit operator Maybe<Decimal>(MaybeDecimal current)
        {
            return current.underlying;
        }

        public U Case<U>(Func<Decimal, U> some, Func<U> none)
        {
            return underlying.Case(some, none);
        }

        public bool HasValue()
        {
            return underlying.HasValue();
        }

        public Decimal Value()
        {
            return underlying.Value();
        }
    }

    public struct MaybeDouble : IMaybe<Double>
    {
        public static MaybeDouble Some(Double value)
        {
            return new MaybeDouble(Maybe.Some(value));
        }

        private Maybe<Double> underlying;
        private MaybeDouble(Maybe<Double> value)
        {
            this.underlying = value;
        }

        public static MaybeDouble operator /(MaybeDouble value2, double value1)
        {
            if (value2.HasValue() && value1 !=0)
            {
                return new MaybeDouble(Maybe.Some(value2.Value() / value1));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator +(MaybeDouble value2, double value1)
        {
            if (value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator *(MaybeDouble value2, double value1)
        {
            if (value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeDouble.None;
        }


        public static MaybeDouble operator /(double value1, MaybeDouble value2)
        {
            if (value2.HasValue() && value2.Value() !=0)
            {
                return new MaybeDouble(Maybe.Some(value1 / value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator +(double value1, MaybeDouble value2)
        {
            if (value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1 + value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator *(double value1, MaybeDouble value2)
        {
            if (value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1 * value2.Value()));
            }
            return MaybeDouble.None;
        }


        public static MaybeDouble operator /(MaybeDouble value1, MaybeDouble value2)
        {
            if (value1.HasValue() && value2.HasValue() && value2.Value() != 0)
            {
                return new MaybeDouble(Maybe.Some(value1.Value() / value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator +(MaybeDouble value1, MaybeDouble value2)
        {
            if(value1.HasValue() && value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1.Value() + value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble operator *(MaybeDouble value1, MaybeDouble value2)
        {
            if (value1.HasValue() && value2.HasValue())
            {
                return new MaybeDouble(Maybe.Some(value1.Value() * value2.Value()));
            }
            return MaybeDouble.None;
        }

        public static MaybeDouble None => new MaybeDouble(Maybe<Double>.None);

        public static implicit operator MaybeDouble(Double current)
        {
            return new MaybeDouble(Maybe.Some(current));
        }

        public static implicit operator MaybeDouble(Maybe<Double> current)
        {
            return new MaybeDouble(current);
        }

        public static implicit operator Maybe<Double>(MaybeDouble current)
        {
            return current.underlying;
        }

        public U Case<U>(Func<Double, U> some, Func<U> none)
        {
            return underlying.Case(some, none);
        }

        public bool HasValue()
        {
            return underlying.HasValue();
        }

        public Double Value()
        {
            return underlying.Value();
        }
    }*/
}
