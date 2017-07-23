using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script
{
    public class ScriptProgramStack : Stack<ScriptData>
    {
        public readonly Stack<ScriptData> AltStack;
        public ScriptProgramStack()
        {
            AltStack = new Stack<ScriptData>();
        }
    }
}
