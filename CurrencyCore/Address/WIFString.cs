namespace CurrencyCore.Address
{
    using Core;
    using CurrencyCore.Coin;
    using System;

    public class WifString
    {
        public readonly HashWithChecksum Value;
        public readonly HashWithoutChecksum ValueWithoutIdentifierOrChecksum;
        private string wifStringCache = null;
        public bool IsCompressed;

        public WifString(HashWithChecksum value, CoinType addressType)
        {
            this.Value = value;

            int offset = 0;
            byte[] bytes = value.GetBytesWithoutChecksum();
            if (bytes[bytes.Length - 1] == 0x01)
            {
                this.IsCompressed = true;
                offset = 1;
            }
            var tempArray = new byte[bytes.Length - (1 + offset)];
            Array.Copy(bytes, 1, tempArray, 0, tempArray.Length);
            this.ValueWithoutIdentifierOrChecksum = new HashWithoutChecksum(tempArray);
        }

        public WifString(HashWithoutChecksum value, CoinType addressType)
        {
            this.ValueWithoutIdentifierOrChecksum = value;

            byte[] rv = new byte[value.GetBytesWithoutChecksum().Length + 2];
            Array.Copy(value.GetBytesWithoutChecksum(), 0, rv, 1, value.GetBytesWithoutChecksum().Length);
            rv[0] = CoinInfo.GetCoinInfo(addressType).GetWifCompressedSignifier();
            rv[rv.Length - 1] = 0x01;

            this.Value = new HashWithChecksum(new HashWithoutChecksum(rv));
        }

        public WifString(string value, CoinType addressType):this(Base58.CreateBase58FromStringWithChecksum(value).Hash, addressType)
        {
            wifStringCache = value;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.wifStringCache)
                ? this.wifStringCache
                : (this.wifStringCache = Base58.CreateBase58(this.Value).ToString());
        }
    }
}

