namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using System.Collections.Generic;
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

    public abstract class ScriptCommand
    {
        public readonly byte OpCode; 
        public readonly ScriptData Data;

        protected ScriptCommand(byte opCode, ScriptData data)
        {
            this.OpCode = opCode;
            this.Data = data;
        }
        
        /// <summary>
        /// the size is any data you push plus one for the opcode
        /// </summary>
        public virtual int Size => this.Data.Size + 1;

        public abstract Stack<ScriptData> Execute(Stack<ScriptData> currentState);

        public virtual void Serialize(byte[] buffer, int offset = 0)
        {
            buffer[offset++] = this.OpCode;
        }
    }
}
