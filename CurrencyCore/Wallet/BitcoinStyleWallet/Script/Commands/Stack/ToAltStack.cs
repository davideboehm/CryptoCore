using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class ToAltStack : ScriptCommand
    {
        public ToAltStack() : base(107)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if (currentState.Count > 0)
            {
                currentState.AltStack.Push(currentState.Pop());
                return ScriptResult.Success;
            }
            return ScriptResult.Fail;
        }
    }
}
