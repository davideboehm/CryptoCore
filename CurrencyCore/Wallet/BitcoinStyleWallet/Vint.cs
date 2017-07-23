using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurrencyCore.Wallet.BitcoinStyleWallet.SerializableExtensions;
namespace CurrencyCore.Wallet.BitcoinStyleWallet
{
    public struct VInt : ISerializable
    {
        private ulong value;
        private byte prefix;
        private int valueLength;

        public int Size { get; private set; }
 

        public static bool operator >(VInt current, UInt64 value)
        {
            return current.value > value;
        }
        public static bool operator <(VInt current, UInt64 value)
        {
            return current.value < value;
        }

        public static implicit operator VInt(UInt16 value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(UInt32 value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(UInt64 value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(byte value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(short value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(int value)
        {
            return new VInt(value);
        }
        public static implicit operator VInt(long value)
        {
            return new VInt((ulong)value);
        }

        public static explicit operator ulong(VInt value)
        {
            return value.value;
        }
        public static explicit operator long(VInt value)
        {
            if(value.value< long.MaxValue)
            {
                return  (long)value.value;
            }
            return 0;
        }
        public static explicit operator int(VInt value)
        {
            if (value.value < int.MaxValue)
            {
                return (int)value.value;
            }
            return 0;
        }

        public VInt(long value) : this((ulong)value)
        {

        }

        public VInt(UInt64 value)
        {
            this.value = value;
            this.prefix = 0x00;
            if (value < 0xFD)
            {
                this.valueLength = 1;
                this.Size = 1;
            }
            else
            { 
                if (value <= 0xFFFF)
                {
                    this.prefix = 0xFD;
                    this.valueLength = 2;
                }
                else if (value <= 0xFFFFFFFF)
                {
                    this.prefix = 0xFE;
                    this.valueLength = 4;
                }
                else
                {
                    this.prefix = 0xFF;
                    this.valueLength = 8;
                }
                this.Size = 1 + this.valueLength;
            }
        }
        private VInt(UInt64 value, byte prefix, int valueLength, int size)
        {
            this.value = value;
            this.prefix = 0x00;
            this.valueLength = valueLength;
            this.Size = size;
        }

        public static VInt? Deserialize(byte[] bytes, int offset = 0)
        {            
            int size = 0;
            UInt64? value = 0;
            byte prefix = 0x00;
            int valueLength = 0;
            if (bytes.Length > 1)
            {
                if (bytes[0] < 0xFD)
                {
                    value = bytes[0];
                    valueLength = 1;
                    size = 1;
                }
                else
                {
                    prefix = bytes[0];
                    switch (prefix)
                    {
                        case 0xFD:
                            {
                                value = bytes.DeserializeUInt16(1);
                                valueLength = 2;
                                break;
                            }
                        case 0xFE:
                            {
                                value = bytes.DeserializeUInt32(1);
                                valueLength = 4;
                                break;
                            }
                        case 0xFF:
                            {
                                value = bytes.DeserializeUInt64(1);
                                valueLength = 8;
                                break;
                            }
                    }
                    size = 1 + valueLength;
                }
                if (value.HasValue)
                {
                    return new VInt(value.Value, prefix, valueLength, size);
                }
            }
            return null;
        }

        public int Serialize(byte[] buffer, int offset)
        {
            buffer[offset] = this.prefix;
            offset += this.valueLength > 1 ? 1 : 0;
            offset += this.value.Serialize(buffer, offset);
            return offset;
        }

        public byte[] GetBytes()
        {
            var result = new byte[this.Size];
            this.Serialize(result, 0);
            return result;
        }
        
    }
}
