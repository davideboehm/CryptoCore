using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.FlowControl
{
    public class Verify : ScriptCommand
    {
        public Verify() : base(105)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            var result = currentState.Count == 0 || currentState.Peek().IsTrue();
            return result ? ScriptResult.Success : ScriptResult.Fail;
        }
    }
}
