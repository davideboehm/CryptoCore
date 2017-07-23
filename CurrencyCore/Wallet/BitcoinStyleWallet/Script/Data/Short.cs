using System;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class Short : ScriptData
    {
        public override int Size => sizeof(short);
        public Short(short byteValue) : base(byteValue.Serialize())
        {
        }
    }
}
