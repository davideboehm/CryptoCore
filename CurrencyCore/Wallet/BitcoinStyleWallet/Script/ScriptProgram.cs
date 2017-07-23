namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script
{
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ScriptProgram : ISerializable
    {
        public int Size
        {
            get
            {
                return Commands.Sum(command => command.Size);
            }
        }

        public List<ScriptCommand> Commands;
        public byte[] GetBytes()
        {
            var size = this.Commands.Sum(entity => entity.Size);
            var result = new byte[size];
            int offset = 0;
            foreach (var entity in this.Commands)
            {
                entity.Serialize(result, offset);
                offset += entity.Size;
            }
            return result;
        }

        public int Serialize(byte[] buffer, int offset = 0)
        {
            var total = 0;
            foreach (var entity in this.Commands)
            {
                total += entity.Serialize(buffer, offset + total);                
            }
            return total;
        }

        public ScriptResult Evaluate()
        {
            ScriptProgramStack stack = new ScriptProgramStack();
            return Evaluate(stack);
        }
        public ScriptResult Evaluate(ScriptProgramStack stack)
        {
            foreach (ScriptCommand currentCommand in Commands)
            {
                if (currentCommand.Execute(stack) == ScriptResult.Fail)
                {
                    return ScriptResult.Fail;
                }
            }
            return (stack.Count == 0 || stack.Peek().IsTrue()) ? ScriptResult.Success : ScriptResult.Fail;
        }
    }
}
