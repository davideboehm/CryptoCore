using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class FromAltStack : ScriptCommand
    {
        public FromAltStack() : base(108)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if (currentState.AltStack.Count > 0)
            {
                currentState.Push(currentState.AltStack.Pop());
                return ScriptResult.Success;
            }
            return ScriptResult.Fail;
        }
    }
}
