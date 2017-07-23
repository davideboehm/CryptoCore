using System;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class Int : ScriptData
    {
        public override int Size => sizeof(int);
        public Int(int byteValue) : base(byteValue.Serialize())
        {
        }
    }
}
