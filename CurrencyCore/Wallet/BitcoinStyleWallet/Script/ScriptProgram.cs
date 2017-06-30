namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script
{
    using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ScriptProgram
    {
        public List<ScriptCommand> Commands;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize()
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
    }
}
