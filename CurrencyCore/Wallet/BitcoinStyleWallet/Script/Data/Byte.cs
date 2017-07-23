namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class Byte : ScriptData
    {
        public override int Size => sizeof(byte);
        public Byte(byte byteValue) : base(new byte[] { byteValue })
        {
        }
    }
}
