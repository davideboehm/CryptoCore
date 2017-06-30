using System;
using System.Text;
namespace Core
{
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
    }
}
