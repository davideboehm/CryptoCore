using CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public class TransactionMessage : Message
    {
        public readonly int Version;
        public readonly ReadOnlyCollection<TxInput> TransactionsIn;
        public readonly ReadOnlyCollection<TxOutput> TransactionsOut;
        public readonly DateTime LockTime;
        public TransactionMessage(int version, IList<TxInput> transactionsIn, IList<TxOutput> transactionsOut, DateTime lockTime) : 
            base("tx", Serialize(version, transactionsIn, transactionsOut, lockTime))
        {
            this.Version = version;
            this.TransactionsIn = new ReadOnlyCollection<TxInput>(transactionsIn);
            this.TransactionsOut = new ReadOnlyCollection<TxOutput>(transactionsOut);
            this.LockTime = lockTime;
        }
        private static byte[] Serialize(int version, IList<TxInput> transactionsIn, IList<TxOutput> transactionsOut, DateTime lockTime)
        {
            var txInSize = transactionsIn.Sum(txin => txin.Size);
            var txOutSize = transactionsOut.Sum(txOut => txOut.Size);
            var txInCountBytes = (VInt)transactionsIn.Count();
            var txOutCountBytes = (VInt)transactionsOut.Count();
            var result = new byte[4 + txInCountBytes.Size + txInSize + txOutCountBytes.Size + txOutSize + 4];
            var offset = version.Serialize(result, 0);
            offset += txInCountBytes.Serialize(result, offset);
            foreach(TxInput txIn in transactionsIn)
            {
                offset += txIn.Serialize(result, offset);
            }
            foreach (TxOutput txout in transactionsOut)
            {
                offset += txout.Serialize(result, offset);
            }
            return result;
        }

        public new static TransactionMessage Deserialize(byte[] rawData, int offset)
        {
            var tuple = DeserializeToTuple(rawData, offset);
            if (!tuple.HasValue)
            {
                return null;
            }

            var (command, payload, networkIndicator, messageBytes) = tuple.Value;

            return null;
        }
    }
}
