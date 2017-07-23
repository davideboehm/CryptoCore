using System;
using System.Linq;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    public class ScriptData : ISerializable
    {
        public static readonly ScriptData Nothing = new ScriptData(null);
        public virtual int Size => this.ByteValue?.Length ?? 0;

        protected virtual byte[] ByteValue { get; set; }

        protected ScriptData()
        {
            this.ByteValue = null;
        }
        protected ScriptData(byte[] byteValue)
        {
            this.ByteValue = byteValue;
        }

        public int Serialize(byte[] buffer, int offset = 0)
        {            
            Buffer.BlockCopy(this.ByteValue, 0, buffer, offset, this.Size);
            return this.Size;
        }
        
        /// <summary>
        /// Determine if the ScriptData represents a true value.
        /// null, empty, and +/- 0  are all considered 0/false
        /// </summary>
        /// <returns>A true for non-zero data</returns>
        public bool IsTrue()
        {
            return !((ByteValue == null) ||
                    (ByteValue.Length == 0) ||
                    (ByteValue.All(currentByte => currentByte == 0)) ||
                    (ByteValue[0] == 0x80 && ByteValue.Skip(1).All(currentByte => currentByte == 0)));
        }
        
        public byte[] GetBytes()
        {
            return this.ByteValue;  
        }
    }
}
