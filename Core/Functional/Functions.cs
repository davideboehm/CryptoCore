using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Functional
{
    public static class Functions
    {
        public static Func<T2> ParitalApply<T, T2>(this Func<T, T2> function, T value)
        {
            return () => function(value);
        }

        public static Func<T2, R> ParitalApply<T, T2, R>(this Func<T, T2, R> function, T value)
        {
            return (T2 value2) => function(value, value2);
        }
        public static Func<T3, R> ParitalApply<T, T2, T3, R>(this Func<T, T2, T3, R> function, T value, T2 value2)
        {
            return (T3 value3) => function(value, value2, value3);
        }
        public static Func<T4, R> ParitalApply<T, T2, T3, T4, R>(this Func<T, T2, T3, T4, R> function, T value, T2 value2, T3 value3)
        {
            return (T4 value4) => function(value, value2, value3, value4);
        }
        public static Func<T5, R> ParitalApply<T, T2, T3, T4, T5, R>(this Func<T, T2, T3, T4, T5, R> function, T value, T2 value2, T3 value3, T4 value4)
        {
            return (T5 value5) => function(value, value2, value3, value4, value5);
        }




        public static Func<T2, T3, R> ParitalApply<T, T2, T3, R>(this Func<T, T2, T3, R> function, T value)
        {
            return (T2 value2, T3 value3) => function(value, value2, value3);
        }

        public static Func<T2, T3, T4, R> ParitalApply<T, T2, T3, T4, R>(this Func<T, T2, T3, T4, R> function, T value)
        {
            return (T2 value2, T3 value3, T4 value4) => function(value, value2, value3, value4);
        }

        public static Func<T2, T3, T4, T5, R> ParitalApply<T, T2, T3, T4, T5, R>(this Func<T, T2, T3, T4, T5, R> function, T value)
        {
            return (T2 value2, T3 value3, T4 value4, T5 value5) => function(value, value2, value3, value4, value5);
        }

        public static Func<T2, T3, T4, T5, T6, R> ParitalApply<T, T2, T3, T4, T5, T6, R>(this Func<T, T2, T3, T4, T5, T6, R> function, T value)
        {
            return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => function(value, value2, value3, value4, value5, value6);
        }

        public static Func<T2, T3, T4, T5, T6, T7, R> ParitalApply<T, T2, T3, T4, T5, T6, T7, R>(this Func<T, T2, T3, T4, T5, T6, T7, R> function, T value)
        {
            return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => function(value, value2, value3, value4, value5, value6, value7);
        }

        public static Func<T2, T3, T4, T5, T6, T7, T8, R> ParitalApply<T, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T, T2, T3, T4, T5, T6, T7, T8, R> function, T value)
        {
            return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => function(value, value2, value3, value4, value5, value6, value7, value8);
        }
    }
}
