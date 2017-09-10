namespace Core
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Hash
    {
        public static byte[] CalculateChecksum(byte[] valueToHash)
        {
            var bcsha256A = SHA256.Create();

            var checksum = bcsha256A.ComputeHash(valueToHash,0,valueToHash.Length);
            return bcsha256A.ComputeHash(checksum, 0, 32);
        }
    }

    public class HashWithoutChecksum:Hash
    {
        
        protected readonly byte[] ValueWithoutChecksum;

        public byte[] GetBytesWithoutChecksum()
        {
            return this.ValueWithoutChecksum;
        }
        public HashWithoutChecksum(byte[] value)
        {
            this.ValueWithoutChecksum = value;
        }
        protected HashWithoutChecksum()
        {
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in this.ValueWithoutChecksum)
            {
                sb.Append(ConvertToBits(b));
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        private static string ConvertToBits(byte currentByte)
        {
            var result = new StringBuilder();
            if (currentByte >= 128)
            {
                result.Insert(0, "1");
                currentByte -= 128;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 64)
            {
                result.Insert(0, "1");
                currentByte -= 64;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 32)
            {
                result.Insert(0, "1");
                currentByte -= 32;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 16)
            {
                result.Insert(0, "1");
                currentByte -= 16;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 8)
            {
                result.Insert(0, "1");
                currentByte -= 8;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 4)
            {
                result.Insert(0, "1");
                currentByte -= 4;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 2)
            {
                result.Insert(0, "1");
                currentByte -= 2;
            }
            else
            {
                result.Insert(0, "0");
            }

            if (currentByte >= 1)
            {
                result.Insert(0, "1");
                currentByte -= 1;
            }
            else
            {
                result.Insert(0, "0");
            }

            return result.ToString();
        }
    }
    public class HashWithChecksum : HashWithoutChecksum
    {
        protected readonly byte[] ValueWithChecksum;

        public byte[] GetBytesWithChecksum()
        {
            return this.ValueWithChecksum;
        }
        protected HashWithChecksum()
        {
        }
        private static byte[] RemoveChecksum(byte[] valueWithChecksum)
        {
            if(valueWithChecksum.Length<=4)
            {
                return null;
            }
            var result = new byte[valueWithChecksum.Length - 4];
            Array.Copy(valueWithChecksum, 0, result, 0, result.Length);
            return result;
        }

        public HashWithChecksum(byte[] value): base(RemoveChecksum(value))
        {
            this.ValueWithChecksum = value;
        }

        public HashWithChecksum(HashWithoutChecksum value)
            : base(value.GetBytesWithoutChecksum())
        {
            var result = new byte[value.GetBytesWithoutChecksum().Length + 4];
            var checksumBytes = CalculateChecksum(value.GetBytesWithoutChecksum());
            Array.Copy(value.GetBytesWithoutChecksum(), 0, result, 0, value.GetBytesWithoutChecksum().Length);
            Array.Copy(checksumBytes, 0, result, value.GetBytesWithoutChecksum().Length, 4);
            this.ValueWithChecksum = result;
        }
    }
}
