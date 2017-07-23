using System;
using System.Collections.ObjectModel;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction
{
    public struct TransactionHash
    { 
        public const int Size = 32;
        public readonly ReadOnlyCollection<byte> Hash;

        private readonly byte[] hash;
        public TransactionHash(byte[] value)
        {
            if(value.Length!= TransactionHash.Size)
            {
                throw new ArgumentException($"Hash must have a length of {TransactionHash.Size}");
            }
            this.hash = value;
            this.Hash = new ReadOnlyCollection<byte>(value);
        }
        public int Serialize(byte[] buffer, int offset =0)
        {
            Buffer.BlockCopy(this.hash, 0, buffer, offset, TransactionHash.Size);
            return TransactionHash.Size;
        }
    }
}
