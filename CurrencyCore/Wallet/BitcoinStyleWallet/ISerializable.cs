using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CurrencyCore.Wallet.BitcoinStyleWallet
{
    public interface ISerializable
    {
        /// <summary>
        /// Put this items bytes into the buffer starting at offset and return the number of bytes written
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        int Serialize(byte[] buffer, int offset = 0);
        byte[] GetBytes();
        int Size { get; }
    }

    public abstract class SerializableBase : ISerializable
    {
        /// <summary>
        /// Put this items bytes into the buffer starting at offset and return the number of bytes written
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public abstract int Serialize(byte[] buffer, int offset = 0);
        public virtual byte[] GetBytes()
        {
            var result = new byte[this.Size];
            this.Serialize(result, 0);
            return result;
        }
        public abstract int Size { get; }
    }
    
    public static class SerializableExtensions
    {
        public static string DeserializeString(this byte[] value, int length, int offset = 0)
        {
            if (value.Length - offset >= length)
            {
                return System.Text.Encoding.UTF8.GetString(value, offset, length);
            }
            return null;
        }

        private static (byte[], int) GetBytesAndOffset(byte[] value, int offset, int size)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Skip(offset).Take(size).Reverse().ToArray();
                offset = 0;
            }
            return (value, offset);
        }

        public static UInt16? DeserializeUInt16(this byte[] value, int offset = 0)
        {
            if (value.Length - offset >= sizeof(UInt16))
            {
                var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(UInt16));
                return BitConverter.ToUInt16(bytes, finalOffset);
            }
            return null;

        }
        public static UInt32? DeserializeUInt32(this byte[] value, int offset = 0)
        {
            if (value.Length - offset >= sizeof(UInt32))
            {
                var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(UInt32));
                return BitConverter.ToUInt32(bytes, finalOffset);
            }
            return null;
        }
        public static UInt64? DeserializeUInt64(this byte[] value, int offset = 0)
        {
            if (value.Length - offset >= sizeof(UInt64))
            {
                var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(UInt64));
                return BitConverter.ToUInt64(bytes, finalOffset);
            }
            return null;
        }

        public static short? DeserializeShort(this byte[] value, int offset = 0)
            {
                if (value.Length - offset >= sizeof(short))
                {
                    var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(short));
            return BitConverter.ToInt16(bytes, finalOffset);
            }
            return null;
        }
        public static int? DeserializeInt(this byte[] value, int offset = 0)
            {
                if (value.Length - offset >= sizeof(int))
                {
                    var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(int));
            return BitConverter.ToInt32(bytes, finalOffset);
        }
            return null;
        }
        public static long? DeserializeLong(this byte[] value, int offset = 0)
        {
            if (value.Length - offset >= sizeof(Int64))
            {
                var (bytes, finalOffset) = GetBytesAndOffset(value, offset, sizeof(Int64));
                return BitConverter.ToInt64(bytes, finalOffset);
            }
            return null;
        }

        public static byte[] Serialize<T>(this List<T> list) where T:ISerializable
        {
            var sizeOfItems = list.Sum(item => item.Size);
            var sizeBytes = ((VInt)list.Count).Serialize();
            var result = new byte[sizeBytes.Length + sizeOfItems];
            Buffer.BlockCopy(sizeBytes, 0, result, 0, sizeBytes.Length);
            int offset = sizeBytes.Length;
            foreach (var item in list)
            {
                item.Serialize(result, offset);
                offset += item.Size;
            }
            return result;
        }

        public static byte[] Serialize(this ValueType value)
        {
            byte[] bytes;
            switch (value)
            {
                case bool b:
                    {
                        bytes = BitConverter.GetBytes(b);
                        break;
                    }
                case char c:
                    {
                        bytes = BitConverter.GetBytes(c);
                        break;
                    }
                case short s:
                    {
                        bytes = BitConverter.GetBytes(s);
                        break;
                    }
                case int i:
                    {
                        bytes = BitConverter.GetBytes(i);
                        break;
                    }
                case uint ui:
                    {
                        bytes = BitConverter.GetBytes(ui);
                        break;
                    }
                case long l:
                    {
                        bytes = BitConverter.GetBytes(l);
                        break;
                    }
                case ushort us:
                    {
                        bytes = BitConverter.GetBytes(us);
                        break;
                    }
                case ulong ul:
                    {
                        bytes = BitConverter.GetBytes(ul);
                        break;
                    }
                case float f:
                    {
                        bytes = BitConverter.GetBytes(f);
                        break;
                    }
                case double d:
                    {
                        bytes = BitConverter.GetBytes(d);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Cannot get bytes for the type {value.GetType().ToString()})");
                    }
            }
            if (BitConverter.IsLittleEndian)
            {
                return bytes;
            }
            else
            {
                return bytes.Reverse().ToArray();
            }
        }
        public static int Serialize(this ValueType value, byte[] buffer, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new ValueType[] { value };
                var size = Marshal.SizeOf(value);
                Buffer.BlockCopy(array, 0, buffer, offset, size);
                return size;
            }
            else
            {
                var values = value.Serialize();
                Buffer.BlockCopy(values, 0, buffer, offset, values.Length);
                return values.Length;
            }
        }
    }
}
