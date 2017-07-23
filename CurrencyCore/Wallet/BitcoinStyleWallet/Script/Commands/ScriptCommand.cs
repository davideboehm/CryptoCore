namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using System.Collections.Generic;
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;
    using System;

    public enum ScriptResult
    {
        Success,
        Fail
    }
    public abstract class ScriptCommand : SerializableBase
    {
        public readonly byte OpCode; 
        public readonly ScriptData Data;

        protected ScriptCommand(byte opCode, ScriptData data)
        {
            this.OpCode = opCode;
            this.Data = data;
        }

        protected ScriptCommand(byte opCode) : this(opCode, ScriptData.Nothing)
        {
        }

        /// <summary>
        /// the size is any data you push plus one for the opcode
        /// </summary>
        public override int Size => this.Data.Size + 1;

        public virtual ScriptResult Execute(ScriptProgramStack currentState)
        {
            return ScriptResult.Success;
        }

        public override int Serialize(byte[] buffer, int offset = 0)
        {
            buffer[offset++] = this.OpCode;
            if (this.Data != null && this.Data != ScriptData.Nothing)
            {
                this.Data.Serialize(buffer, offset);
            }
            return 1 + this.Data.Size;
        }        
    }
}
