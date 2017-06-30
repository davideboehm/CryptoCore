namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using System;
    using System.Collections.Generic;
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

    public class CheckSig : ScriptCommand
    {
        public CheckSig() : base(172, null)
        {
        }

        public override Stack<ScriptData> Execute(Stack<ScriptData> currentState)
        {
            throw new NotImplementedException();
        }        
    }
}
