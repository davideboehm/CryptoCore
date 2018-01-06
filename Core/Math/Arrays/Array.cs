using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Arrays
{
    public static class ArrayUtils
    {
        public static double Norm(this double[] array)
        {
            var sum = array.Select((value) => value * value).Sum();
            return Math.Sqrt(sum);
        }

        public static double Dot(this double[] array1, double[] array2)
        {
            if(array1.Length != array1.Length)
            {
                throw new ArgumentException("The arrays need to be the same length");
            }
            var sum = 0.0;
            for(int i=0;i<array1.Length;i++)
            {
                sum += array1[i] * array2[i];
            }
            return sum;
        }

        public static double[] Subtract(this double[] array1, double value)
        {
            var result = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = array1[i] - value;
            }
            return result;
        }
        public static double[] Subtract(this double[] array1, double[] array2)
        {
            if (array1.Length != array1.Length)
            {
                throw new ArgumentException("The arrays need to be the same length");
            }
            var result = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = array1[i] - array2[i];
            }
            return result;
        }
        public static double[] Multiply(this double[] array1, double value)
        {
            var result = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = array1[i] * value;
            }
            return result;
        }
        public static double[] Multiply(this double value, double[] array1)
        {
            return array1.Multiply(value);
        }
    }
}

