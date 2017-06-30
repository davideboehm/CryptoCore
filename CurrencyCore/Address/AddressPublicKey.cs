namespace CurrencyCore.Address
{
    using System;
    using System.Security.Cryptography;
    using Core;

    public class AddressPublicKey
    {
        public Base58 Value;
        public bool IsCompressed;
        private String base58StringCached;
        public String Base58String
        {
            get { return this.base58StringCached ?? (this.base58StringCached = this.Value.ToString()); }
        }
        public AddressPublicKey(HashWithChecksum hash)
        {
            if(hash.GetBytesWithoutChecksum().Length==33)
            {
                this.IsCompressed = true;
            }
            this.Value = Base58.CreateBase58(hash);
        }
        public AddressPublicKey(Base58 base58Representation)
            : this(base58Representation.Hash)
        {
        }

        public bool IsValid()
        {
            var sha256 = SHA256.Create();


            var checksum = sha256.ComputeHash(this.Value.Hash.GetBytesWithoutChecksum());
            checksum = sha256.ComputeHash(checksum, 0, 32);

            for (var i = 0; i < 4; i++)
            {
                if (checksum[i] !=
                    this.Value.Hash.GetBytesWithChecksum()[this.Value.Hash.GetBytesWithChecksum().Length - 4 + i])
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return this.Base58String;
        }
    }
}
