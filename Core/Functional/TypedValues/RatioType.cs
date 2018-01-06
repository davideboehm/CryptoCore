using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Functional.TypedValues
{
    public class RatioType : NumericType
    {
        private string numerator;
        private string denominator;
        public RatioType(string numerator, string denominator): base(new TypeStruct(numerator, 1), new TypeStruct(denominator,-1))
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }

        public override string ToString()
        {
            return $"{numerator} per {denominator}";
        }
    }
    public class RatioType<T> : RatioType
    {
        public RatioType(T numerator, T denominator) : base(numerator.ToString(), denominator.ToString())
        {
        }
    }

    public class RatioType<T,U> : RatioType
    {
        public RatioType(T numerator, U denominator) :base(numerator.ToString(), denominator.ToString())
        {
        }
    }
}
