namespace CurrencyCore.Address
{
    using System;
    using CurrencyCore.Coin;
    using Core;

    public class PublicAddress
    {
        public CoinType AddressType;
        public readonly bool IsHex;
        protected Lazy<AddressPublicKey> LazyPublicKey;

        public PublicAddress()
        {
        }

        public PublicAddress(AddressPublicKey key, CoinType addressType)
        {
            this.LazyPublicKey = new Lazy<AddressPublicKey>(() => key);
            this.AddressType = addressType;
        }
        public PublicAddress(String key, CoinType addressType)
        {
            this.IsHex = (key.StartsWith("0x") || key.StartsWith("0X"));
            this.LazyPublicKey = new Lazy<AddressPublicKey>(() =>
            {            
                return this.IsHex ?
                    new AddressPublicKey(new HashWithChecksum(ByteArrayUtil.HexStringToByteArray(key))) : 
                    new AddressPublicKey(Base58.CreateBase58FromStringWithChecksum(key));
            });
            this.AddressType = addressType;
        }

        public AddressPublicKey PublicKey
        {
            get { return this.LazyPublicKey.Value; }
        }

        public Byte[] GetHash160()
        {
            return this.PublicKey.Value.Hash.GetBytesWithChecksum();
        }
        public override string ToString()
        {
            if (this.IsHex)
            {
                return this.AsHexString();
            }
            return this.PublicKey.ToString();
        }
        public string AsHexString()
        {
            return this.PublicKey.AsHexString();
        }
    }
}
