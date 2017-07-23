using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.Crypto
{
    public class Hash160: ScriptCommand
    {
        public Hash160():base(169)
        {
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            if (currentState.Count > 0)
            {
                var data = currentState.Pop().GetBytes();
                var sha256 = SHA256.Create();
                var ripemd160 = RIPEMD160.Create();
                var value = ripemd160.ComputeHash(sha256.ComputeHash(data, 0, data.Length), 0, 32);
                currentState.Push(new Bytes(value));
            }
            return ScriptResult.Success;
        }
    }
}
