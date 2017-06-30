namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class BoolData : ScriptData
    {
        public BoolData(bool value):base( new byte[] { value ? (byte)1 : (byte)0 })
        {
        }
    }
}
