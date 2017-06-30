namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class ScriptData
    {
        public int Size => this.byteValue.Length;
        protected readonly byte[] byteValue;

        public ScriptData(byte[] byteValue)
        {
            this.byteValue = byteValue;
        }

        public void Serialize(byte[] buffer, int offset = 0)
        {
            //values are little endian so put the values in backwards
            for (int i = this.Size - 1; i >= 0; i--)
            {
                buffer[offset++] = this.byteValue[i];
            }
        }
    }
}
