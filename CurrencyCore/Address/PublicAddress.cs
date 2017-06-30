namespace CurrencyCore.Address
{
    using System;
    using CurrencyCore.Coin;
    using Core;

    public class PublicAddress
    {
        public CoinType AddressType;

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
            this.LazyPublicKey = new Lazy<AddressPublicKey>(() => new AddressPublicKey(Base58.CreateBase58FromStringWithChecksum(key)));
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
            return this.PublicKey.ToString();
        }
    }
}
