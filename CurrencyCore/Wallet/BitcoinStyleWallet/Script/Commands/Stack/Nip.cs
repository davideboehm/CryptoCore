using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Stack
{
    public class Nip : ScriptCommand
    {
        public Nip() : base(119)
        {
        }

        /// <summary>
        /// Remove the second item on the stack
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if(currentState.Count>1)
            {
                var top = currentState.Pop();
                currentState.Pop();
                currentState.Push(top);
            }            

            return ScriptResult.Success;
        }
    }
}
