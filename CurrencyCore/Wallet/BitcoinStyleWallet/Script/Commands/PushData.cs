namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands
{
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;
    using System.Collections.Generic;
    using System;

    public class PushData : ScriptCommand
    {
        public PushData(ScriptData data) 
            : base(data.Size < byte.MaxValue ? 
                  (byte)76 : (data.Size < short.MaxValue? (byte)77 : (byte)78), data)
        {
        }

        /// <summary>
        /// This command is has additional bytes to describe how large the data is.
        /// </summary>
        public override int Size => base.Size + 1 + (this.Data.Size < short.MaxValue ? 1 : 3);
                                    

        public override Stack<ScriptData> Execute(Stack<ScriptData> currentState)
        {
            currentState.Push(this.Data);
            return currentState;
        }
        public override void Serialize(byte[] buffer, int offset = 0)
        {
            int index = 0 + offset;
            buffer[index++] = this.OpCode;
            var count = this.Data.Size;
            if(count<byte.MaxValue)
            {
                buffer[index++] = (byte)(count);
            }
            else if(count < short.MaxValue)
            {
                var value = BitConverter.GetBytes((short)count);
                buffer[index++] = value[1];
                buffer[index++] = value[0];
            }
            else
            {
                var value = BitConverter.GetBytes(count);
                buffer[index++] = value[3];
                buffer[index++] = value[2];
                buffer[index++] = value[1];
                buffer[index++] = value[0];
            }
            this.Data.Serialize(buffer, index);            
        }
    }
}
