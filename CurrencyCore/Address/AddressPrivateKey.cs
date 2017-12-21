namespace CurrencyCore.Address
{
    using System;
    using System.Security.Cryptography;
        using Core;
    using CurrencyCore.Coin;

    public class AddressPrivateKey
    {
        public HashWithoutChecksum Value;
        public readonly CurrencyType AddressType;
        public bool IsCompressed;

        public AddressPrivateKey(HashWithoutChecksum value, CurrencyType addressType, bool isCompressed)
        {
            this.Value = value;
            this.AddressType = addressType;
            this.IsCompressed = isCompressed;
        }

        public AddressPrivateKey(WifString wifString, CurrencyType addressType)
            : this(wifString.ValueWithoutIdentifierOrChecksum, addressType,wifString.IsCompressed)
        {
            this.wifString = wifString.ToString();
        }

        public AddressPublicKey ComputePublicKey()
        {
            Base58 result;

            //Generate the corresponding public key with the private key 
            //(65 bytes, 1 byte 0x04, 32 bytes corresponding to X coordinate, 32 bytes corresponding to Y coordinate)
            var ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
            var point = ps.G;

            var Db = new Org.BouncyCastle.Math.BigInteger(1, this.Value.GetBytesWithoutChecksum());
            var dd = point.Multiply(Db);

            byte[] pubaddrByteArray;

            var X = dd.Normalize().XCoord.ToBigInteger();
            var Y = dd.Normalize().YCoord.ToBigInteger();

            if (this.IsCompressed)
            {
                dd = ps.Curve.CreatePoint(X, Y);
                pubaddrByteArray =  dd.GetEncoded(true);
            }
            else
            {
                pubaddrByteArray = new byte[65];
                var byteArrayX = X.ToByteArray();
                var byteArrayY = Y.ToByteArray();
                Array.Copy(byteArrayY, 0, pubaddrByteArray, 64 - byteArrayY.Length + 1, byteArrayY.Length);

                Array.Copy(byteArrayX, 0, pubaddrByteArray, 32 - byteArrayX.Length + 1, byteArrayX.Length);
                pubaddrByteArray[0] = 4;
            }
            
            //Perform SHA-256 hashing on the public key
            using (var sha256 = SHA256.Create())
            {
                var sha256Hash1 = sha256.ComputeHash(pubaddrByteArray);

                byte[] ripemd160Hash;
                //Perform RIPEMD-160 hashing on the result of SHA-256
                using (var ripemd160 = RIPEMD160.Create())
                {
                    ripemd160Hash = ripemd160.ComputeHash(sha256Hash1);
                }

                //Add version byte in front of RIPEMD-160 hash
                var ripemd160HashWithVersionByte = new byte[ripemd160Hash.Length + 1];
                Array.Copy(ripemd160Hash, 0, ripemd160HashWithVersionByte, 1, ripemd160Hash.Length);


                ripemd160HashWithVersionByte[0] = CryptoCurrency.GetCryptoCurrency(this.AddressType).GetAddressSignifier();

                var finishedHashWithoutChecksum = new HashWithoutChecksum(ripemd160HashWithVersionByte);
                var finishedHashWithChecksum = new HashWithChecksum(finishedHashWithoutChecksum);
                result = Base58.CreateBase58(finishedHashWithChecksum);
            }

            return result == null ? null : new AddressPublicKey(result);
        }

        private string wifString = null;
        public string WifString
        {
            get
            {
                return !string.IsNullOrEmpty(this.wifString)
                    ? this.wifString
                    : (this.wifString = new WifString(this.Value, this.AddressType).ToString());
            }
        }

        public override string ToString()
        {
            return this.WifString;
        }
    }
}
