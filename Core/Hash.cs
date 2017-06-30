namespace Core
{
    using System;
    using System.Security.Cryptography;

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
