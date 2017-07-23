using CurrencyCore.Wallet.BitcoinStyleWallet.Script;
namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages.Transaction
{
    public struct Script
    {
        public readonly VInt length;
        public ScriptProgram SignatureScript;

        public int Size()
        {
           return length.Size + SignatureScript.Size;            
        }
        public int Serialize(byte[] buffer, int offset = 0)
        {
            var total = length.Serialize(buffer, offset);
            total += SignatureScript.Serialize(buffer, offset + total);
            return total;
        }
    }
}
