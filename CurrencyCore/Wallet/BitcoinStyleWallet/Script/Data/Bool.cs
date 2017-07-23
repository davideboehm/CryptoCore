namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class BoolData : ScriptData
    {
        public BoolData(bool value):base( value ? new byte[] {1} : new byte[0])
        {
        }
        public static readonly BoolData False = new BoolData(false);
        public static readonly BoolData True = new BoolData(true);
        public override int Size => 1;
    }
}
