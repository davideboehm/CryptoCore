using System;
using System.Net;

using static CurrencyCore.Wallet.BitcoinStyleWallet.SerializableExtensions;
namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public struct PeerInfo : ISerializable
    {
        public const int Size = 30;
        public DateTime LastSeen;
        public UInt64 Features;
        public IPAddress IPAddress;
        public UInt16 Port;
        private Lazy<byte[]> bytes;

        public byte[] Bytes
        {
            get
            {
                return bytes.Value;
            }
        }

        int ISerializable.Size => PeerInfo.Size;

        /// <summary>
        /// Copies the bytes that represent this PeerInfo into the buffer if there is room for it
        /// </summary>
        /// <param name="buffer">the buffer that the bytes will be copied into</param>
        /// <param name="offset">where in the buffer to start copying the bytes</param>
        public int Serialize(byte[] buffer, int offset)
        {
            if (this.bytes.IsValueCreated)
            {
                Array.Copy(this.Bytes, 0, buffer, offset, PeerInfo.Size);
            }
            else
            {
                ConstructBytes(buffer, this.LastSeen, this.Features, this.IPAddress, this.Port, offset);
            }
            return PeerInfo.Size;
        }

        /// <summary>
        /// Calculate the bytes that represent the passed in PeerInfo data and copy them into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="lastSeen"></param>
        /// <param name="features"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="offset"></param>
        private static void ConstructBytes(byte[] buffer, DateTime lastSeen, UInt64 features, IPAddress address, UInt16 port, int offset = 0)
        {
            if (buffer.Length >= offset + PeerInfo.Size)
            {
                ((uint)(lastSeen - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).Serialize(buffer, offset + 0);
                features.Serialize(buffer, offset + 4);
                Array.Copy(address.GetAddressBytes(), 0, buffer, offset + 12, 16);
                port.Serialize(buffer, offset + 28);
            }
        }

        public PeerInfo(DateTime lastSeen, UInt64 features, IPAddress address, UInt16 port)
        {
            this.LastSeen = lastSeen;
            this.Features = features;
            this.IPAddress = address;
            this.Port = port;

            bytes = new Lazy<byte[]>(()=>
            {
                var result = new byte[30];
                ConstructBytes(result, lastSeen, features, address, port);
                return result;
             });
        }
        public static PeerInfo? Deserialize(byte[] rawData, int offset = 0)
        {
            if (rawData.Length >= offset + PeerInfo.Size)
            {
                try
                {
                    var lastSeen = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(0, 0, rawData.DeserializeInt(offset) ?? 0);

                    var features = rawData.DeserializeUInt64(offset + 4) ?? 0;

                    var buffer = new byte[16];
                    Buffer.BlockCopy(rawData, offset + 12, buffer, 0, 16);

                    var IPAddress = new IPAddress(buffer);

                    var Port = rawData.DeserializeUInt16(offset + 28) ?? 0;
                    return new PeerInfo(lastSeen, features, IPAddress, Port);
                }
                catch
                {

                }
                return null;
            }
            return null;
        }
        
        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
