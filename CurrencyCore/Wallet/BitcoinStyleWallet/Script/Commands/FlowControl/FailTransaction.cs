using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.FlowControl
{
    public class FailTransaction : ScriptCommand
    {
        public FailTransaction() : base(106)
        {
        }

        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            return ScriptResult.Fail;
        }
    }
}


