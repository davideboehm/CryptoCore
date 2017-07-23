using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class Duplicate : ScriptCommand
    {
        public Duplicate() : base(118)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if (currentState.Count > 0)
            {
              currentState.Push(currentState.Peek());                
            }

            return ScriptResult.Success;
        }
    }
}
