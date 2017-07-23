using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class IfDuplicate : ScriptCommand
    {
        public IfDuplicate() : base(115)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if (currentState.Count > 0)
            {
                var top = currentState.Peek();
                if (top.IsTrue())
                {
                    currentState.Push(top);                
                }
            }

            return ScriptResult.Success;
        }
    }
}
