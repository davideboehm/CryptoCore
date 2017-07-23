namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.PushCommands
{
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;
    using System;

    public abstract class PushData : ScriptCommand
    {
        protected PushData(ScriptData data) : base((byte)data.Size, data)
        {
            if(data.Size<1 || data.Size >75)
            {
                throw new ArgumentException("DataPushed via this command must be between 1 and 75 inclusive bytes long");
            }
        }

        protected PushData(byte opCode, ScriptData data) : base(opCode, data)
        {
        }
                 
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            currentState.Push(this.Data);
            return ScriptResult.Success;
        }        
    }

    public class PushData1 : PushData
    {
        public PushData1(Data.Byte data) : base(76, data)
        {
        }

        public override int Serialize(byte[] buffer, int offset = 0)
        {
            buffer[offset++] = this.OpCode;
            var count = this.Data.Size;
            buffer[offset++] = (byte)(count);
            this.Data.Serialize(buffer, offset);
            return this.Size;
        }
        public override int Size => base.Size + 1;
    }

    public class PushData2 : PushData
    {
        public PushData2(Data.Short data) : base(77, data)
        {
        }

        public override int Serialize(byte[] buffer, int offset = 0)
        {
            buffer[offset++] = this.OpCode;
            var count = this.Data.Size;
            offset += ((short)count).Serialize(buffer,offset);
            this.Data.Serialize(buffer, offset);
            return this.Size;
        }

        public override int Size => base.Size + 2;
    }

    public class PushData4 : PushData
    {
        public PushData4(Data.Int data) : base(78, data)
        {
        }

        public override int Serialize(byte[] buffer, int offset = 0)
        {
            buffer[offset++] = this.OpCode;
            var count = this.Data.Size;
            offset += count.Serialize(buffer,offset);

            this.Data.Serialize(buffer, offset);
            return this.Size;
        }
        public override int Size => base.Size + 4;
    }
}
