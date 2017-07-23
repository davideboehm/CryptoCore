using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction
{
    public struct Output
    {
        public const int Size = TransactionHash.Size + sizeof(uint);
        public readonly TransactionHash Hash;
        public readonly uint Index;

        public Output(TransactionHash hash, uint index)
        {
            this.Hash = hash;
            this.Index = index;
        }
        public int Serialize(byte[] buffer, int offset = 0)
        {
            offset += this.Hash.Serialize(buffer, offset);
            offset += this.Index.Serialize(buffer, offset);
            return Output.Size;
        }
    }
}
