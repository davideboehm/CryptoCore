namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using System;
    using System.Collections.Generic;
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

    public class CheckSigVerify : ScriptCommand
    {
        public CheckSigVerify() : base(173, null)
        {
        }

        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            var result = currentState.Count == 0 || currentState.Peek().IsTrue();
            return result ? ScriptResult.Success : ScriptResult.Fail;
        }
    }
}
