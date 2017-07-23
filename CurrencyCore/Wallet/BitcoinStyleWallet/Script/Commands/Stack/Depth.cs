using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class Depth : ScriptCommand
    {
        public Depth() : base(116)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            currentState.Push(new Int(currentState.Count));

            return ScriptResult.Success;
        }
    }
}
