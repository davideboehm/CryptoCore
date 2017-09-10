using System;
using System.Text;
namespace Core
{
    using System.Collections;
    using System.Security.Cryptography;

    public static class RandomGeneratorUtil
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        public static ulong NextLong()
        {
            ulong result = 0;
            var randomBytes = new byte[8];
            Rng.GetBytes(randomBytes);

            foreach (var b in randomBytes)
            {
                result += b;
                result *= 256;
            }

            return result;
        }

        public static byte[] GetByteArray(int lengthOfArray)
        {
            var result = new byte[lengthOfArray];

            Rng.GetBytes(result);

            return result;
        }

        public static byte[] GetByteArrayWithValuesSet(int length, params int[] bitsSet)
        {
            BitArray array = new BitArray(length * 8);

            foreach (var currentBit in bitsSet)
            {
                array.Set(currentBit, true);
            }

            return ConvertToByteArray(array);
        }
     
        private static byte[] ConvertToByteArray(BitArray bits)
        {
            if (bits.Count % 8 != 0)
            {
                throw new ArgumentException("illegal number of bits");
            }

            var result = new byte[bits.Count / 8];
            for (int i = 0; i < bits.Length / 8; i++)
            {
                byte b = 0;
                if (bits.Get(i * 8 + 0))
                {
                    b++;
                }
                if (bits.Get(i * 8 + 1))
                {
                    b += 2;
                }
                if (bits.Get(i * 8 + 2))
                {
                    b += 4;
                }
                if (bits.Get(i * 8 + 3))
                {
                    b += 8;
                }
                if (bits.Get(i * 8 + 4))
                {
                    b += 16;
                }
                if (bits.Get(i * 8 + 5))
                {
                    b += 32;
                }
                if (bits.Get(i * 8 + 6))
                {
                    b += 64;
                }
                if (bits.Get(i * 8 + 7))
                {
                    b += 128;
                }
                result[i] = b;
            }
            return result;
        }
    }
}
