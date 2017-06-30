namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Text;

    public class Base58
    {
        private const string B58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        public readonly HashWithChecksum Hash;

        protected Base58(HashWithChecksum hash)
        {
            this.Hash = hash;
        }

        private string CalculateStringRepresentation()
        {
            var result = new StringBuilder();
            var value = this.Hash.GetBytesWithChecksum();
            if(value[0]>=0x80)
            {
                var temp = new byte[value.Length + 1];
                Array.Copy(value, 0, temp, 1, value.Length);
                value = temp;
            }
            var addrremain = new BigInteger(value.Reverse().ToArray());

            var big0 = new BigInteger(0);
            var big58 = new BigInteger(58);

            while (addrremain.CompareTo(big0) > 0)
            {
                BigInteger remainder;
                addrremain = BigInteger.DivRem(addrremain, big58, out remainder);
                var digit = Convert.ToInt32(remainder.ToString());
                result.Insert(0, B58[digit]);
            }

            // handle leading zeroes
            for (var i = 0; this.Hash.GetBytesWithChecksum()[i] == 0; i++)
            {
                result.Insert(0, "1");
            }

            return result.ToString();
        }

        private string stringRepresentation = null;

        public override string ToString()
        {
            return (!string.IsNullOrEmpty(this.stringRepresentation)
                ? this.stringRepresentation
                : (this.stringRepresentation = this.CalculateStringRepresentation()));
        }

        public static Base58 CreateBase58FromString(string base58String)
        {
            var byteArray = CalculateByteArrayFromString(base58String);
            if(byteArray==null)
            {
                return null;
            }

            var bb = new byte[byteArray.Length - 4];
            Array.Copy(byteArray, bb, byteArray.Length);

            var bcsha256A = SHA256.Create();
            var hash = bcsha256A.ComputeHash(bb, 0, bb.Length);
            hash = bcsha256A.ComputeHash(hash, 0, 32);
            
            for (var i = 0; i < 4; i++)
            {
               if(byteArray[bb.Length + i] != hash[i])
               {
                   return CreateBase58(new HashWithoutChecksum(byteArray));
               }
            }
            return CreateBase58(new HashWithChecksum(byteArray));
        }

        public static Base58 CreateBase58(HashWithoutChecksum hash)
        {
            var withByteArray = new byte[hash.GetBytesWithoutChecksum().Length + 4];
            Array.Copy(hash.GetBytesWithoutChecksum(), withByteArray, hash.GetBytesWithoutChecksum().Length);
            var bcsha256A = SHA256.Create();
            var checksum = bcsha256A.ComputeHash(hash.GetBytesWithoutChecksum(), 0, hash.GetBytesWithoutChecksum().Length);
            checksum = bcsha256A.ComputeHash(checksum, 0, 32);

            for (var i = 0; i < 4; i++)
            {
                withByteArray[hash.GetBytesWithoutChecksum().Length + i] = checksum[i];
            }

            return new Base58(new HashWithChecksum(withByteArray));
        }

        public static Base58 CreateBase58(HashWithChecksum hash)
        {
            if (hash == null || hash.GetBytesWithoutChecksum() == null)
            {
                return null;
            }
            return new Base58(hash);
        }

        private static byte[] CalculateByteArrayFromString(string stringValue)
        {
            try
            {
                var value = new BigInteger(0);

                foreach (var c in stringValue)
                {
                    if (B58.IndexOf(c) != -1)
                    {
                        value = BigInteger.Multiply(value, new BigInteger(58));
                        value = BigInteger.Add(value, new BigInteger(B58.IndexOf(c)));
                    }
                    else
                    {
                        return null;
                    }
                }
                var temp = value.ToByteArray().Reverse();
                var enumerable = temp as IList<byte> ?? temp.ToList();
                var skipCount = enumerable.TakeWhile(c => c == 0).Count();
                var ba = enumerable.Skip(skipCount).ToArray();

                var i = stringValue.TakeWhile(c => c == '1').Count();
                if (i > 0)
                {
                    var bb = new byte[ba.Length + i];
                    Array.Copy(ba, 0, bb, i, ba.Length);
                    ba = bb;
                }
                return ba;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Base58 CreateBase58FromStringWithoutChecksum(string base58StringWithoutChecksum)
        {
            var byteArray = CalculateByteArrayFromString(base58StringWithoutChecksum);
            return byteArray == null ? null : Base58.CreateBase58(new HashWithoutChecksum(byteArray));
        }

        public static Base58 CreateBase58FromStringWithChecksum(string base58StringWithChecksum)
        {
            var byteArray = CalculateByteArrayFromString(base58StringWithChecksum);
            return byteArray == null ? null : Base58.CreateBase58(new HashWithChecksum(byteArray));
        }
    }
}
