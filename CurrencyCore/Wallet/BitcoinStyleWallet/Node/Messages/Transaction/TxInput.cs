using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction
{
    public struct TxInput : ISerializable
    {
        public readonly Output PreviousOutput;
        public readonly Script ScriptData;
        public readonly uint Sequence;
        public int Size =>  Output.Size + ScriptData.Size() + sizeof(uint);      

        public int Serialize(byte[] buffer, int offset = 0)
        {
            return 0;
        }
        public byte[] GetBytes()
        {
            var result = new byte[this.Size];
            this.Serialize(result);
            return result;
        }
    }
}
