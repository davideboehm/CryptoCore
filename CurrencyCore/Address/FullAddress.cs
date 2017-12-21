
namespace CurrencyCore.Address
{
    using System;
    using CurrencyCore.Coin;
        using Core;

    public class FullAddress : PublicAddress
    {
        public AddressPrivateKey PrivateKey;

        private void Initialize(AddressPrivateKey privateKey, AddressPublicKey publickKey, bool forcePublicKeyGeneration)
        {
            this.AddressType = privateKey.AddressType;
            this.PrivateKey = privateKey;

            if (forcePublicKeyGeneration)
            {
                var publicKey = publickKey ?? this.PrivateKey.ComputePublicKey();
                this.LazyPublicKey = new Lazy<AddressPublicKey>(() => publicKey);
            }
            else
            {
                this.LazyPublicKey = new Lazy<AddressPublicKey>(() => publickKey ?? this.PrivateKey.ComputePublicKey());
            }
        }

        public FullAddress(CurrencyType coinType, bool forcePublicKeyGeneration = false)
        {
            AddressPrivateKey privateKey = new AddressPrivateKey(new HashWithoutChecksum(RandomGeneratorUtil.GetByteArray(32)), coinType, true);
            this.Initialize(privateKey, null, forcePublicKeyGeneration);
        }

        /// <summary>
        /// Constructs a full address from a private key.
        /// </summary>
        public FullAddress(AddressPrivateKey privateKey, bool forcePublicKeyGeneration = false) 
        {
            this.Initialize(privateKey, null, forcePublicKeyGeneration);
        }
        
        /// <summary>
        /// Constructs a full address from a private key.
        /// </summary>
        public FullAddress(AddressKeyPair keyInfo)
        {
            this.Initialize(keyInfo.PrivateKey, keyInfo.PublicKey, true);
        }

        public override string ToString()
        {
            return this.PublicKey.ToString();
        }
    }
}
