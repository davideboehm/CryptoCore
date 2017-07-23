using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.FlowControl
{
    public class If : ScriptCommand
    {
        private readonly ScriptBranch trueBranch;
        private readonly ScriptBranch falseBranch;
        private readonly bool Negate;
        public If(ScriptBranch trueBranch, ScriptBranch falseBranch = null) : this(99, false, trueBranch, falseBranch)
        {
        }

        protected If(byte opCode, bool negate, ScriptBranch trueBranch, ScriptBranch falseBranch = null) : base(opCode, CombineBranchData(trueBranch, falseBranch))
        {
            this.Negate = negate;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }

        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            var value = currentState.Pop();
            if(this.Negate ^ value.IsTrue())
            {
               return trueBranch.Execute(currentState);
            }
            else if(falseBranch != null)
            {
                return falseBranch.Execute(currentState);
            }
            return ScriptResult.Success;
        }

        private static ScriptData CombineBranchData(ScriptBranch firstBranch, ScriptBranch secondBranch)
        {
            var size = firstBranch.Size + 1 + (secondBranch != null ? secondBranch.Size + 1 : 0);
            var buffer = new byte[size];
            firstBranch.Serialize(buffer, 0);
            if (secondBranch != null)
            {
                //add else
                buffer[firstBranch.Size] = 103;
                secondBranch.Serialize(buffer, firstBranch.Size + 1);
            }
            //add endif
            buffer[size - 1] = 104;
            return new Bytes(buffer);
        }
    }
}
