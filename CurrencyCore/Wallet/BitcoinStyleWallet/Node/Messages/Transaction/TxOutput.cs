using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction
{
    public struct TxOutput : ISerializable
    {
        public readonly UInt64 TransactionValue;
        public readonly Script Script;
        public int Size => sizeof(UInt64) + Script.Size();

        public byte[] GetBytes()
        {
            var result = new byte[this.Size];
            this.Serialize(result);
            return result;
        }

        public int Serialize(byte[] buffer, int offset = 0)
        {
            offset += this.TransactionValue.Serialize(buffer, offset);
            offset += Script.Serialize(buffer, offset);
            return TransactionHash.Size;
        }        
    }
}
