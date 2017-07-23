namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using System;
    using System.Collections.Generic;
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

    public class CheckSig : ScriptCommand
    {
        public CheckSig() : base(172)
        {
        }

        public override ScriptResult Execute(ScriptProgramStack currentState)
        {/*
            var pubKey = currentState.Pop();
            var sig = currentState.Pop();
            var DERSignature = new byte[sig.Size-1];
            var sigData = sig.Serialize();
            Array.Copy(sigData, 0, DERSignature, 0, DERSignature.Length);
            var hashType = sigData[sigData.Length - 1];

            var subscript = ScriptData.Nothing;

            */


            return ScriptResult.Success;
        }        
    }
}
