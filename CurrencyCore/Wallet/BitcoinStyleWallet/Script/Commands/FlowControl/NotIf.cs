using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.FlowControl
{
    public class NotIf : If
    {
        public NotIf(ScriptBranch trueBranch, ScriptBranch falseBranch = null) : base(100, true, trueBranch, falseBranch)
        {
        }
    }
}
