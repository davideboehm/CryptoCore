using System;
using System.Linq;
using System.Security.Cryptography;


namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public class Message : SerializableBase
    {
        protected readonly string command;
        protected readonly byte[] messageBytes;
        protected const uint defaultNetworkIndicator = 0xD9B4BEF9;
        protected readonly uint networkIndicator;
        protected readonly byte[] payload;

        public override int Size => this.messageBytes.Length;

        protected Message(string command) : this(command, new byte[0])
        {
        }

        protected Message(string command, byte[] payload)
        {
            if (command.Length > 12)
            {
                throw new ArgumentException($"{nameof(command)} must be 12 or less characters long");
            }

            this.command = command;
            this.payload = payload;
            this.networkIndicator = defaultNetworkIndicator;

            //create a buffer for the message
            messageBytes = new byte[24 + payload.Length];

            //the first 4 bytes are the network indicator
            networkIndicator.Serialize(messageBytes, 0);

            //the next 12 bytes are the command/message type
            Array.Copy(command.Select(currentChar => (byte)currentChar).ToArray(), 0, messageBytes, 4, command.Length);

            //the next 4 bytes are the size of the payload
            payload.Length.Serialize(messageBytes, 16);

            //the next 4 bytes are the checksum of the payload
            var sha256 = SHA256.Create();
            var checksum = sha256.ComputeHash(sha256.ComputeHash(payload, 0, payload.Length), 0, 32);
            Array.Copy(checksum, 0, messageBytes, 20, 4);

            //the rest of the buffer is filled with the payload
            Array.Copy(payload, 0, messageBytes, 24, payload.Length);
        }
        
        protected Message(string command, byte[] payload, uint networkIndicator, byte[] messageBytes)
        {
            this.command = command;
            this.payload = payload;
            this.networkIndicator = networkIndicator;
            this.messageBytes = messageBytes;
        }

        protected static (string command, byte[] payload, uint networkIndicator, byte[] messageBytes)? DeserializeToTuple(byte[] rawData, int offset = 0)
        {
            if (rawData.Length - offset >= 24)
            {
                var messageBytes = rawData;

                var networkIndicator = rawData.DeserializeUInt32(offset) ?? 0;

                var command = rawData.DeserializeString(12, offset + 4);

                var payloadLength = rawData.DeserializeUInt32(offset + 16) ?? 0;

                var payload = new byte[payloadLength];
                Array.Copy(rawData, offset + 24, payload, 0, payloadLength);

                var bcsha256A = SHA256.Create();
                var calculatedChecksumBytes = bcsha256A.ComputeHash(bcsha256A.ComputeHash(payload, 0, payload.Length), 0, 32);

                if (calculatedChecksumBytes[0] != rawData[offset + 20] ||
                    calculatedChecksumBytes[1] != rawData[offset + 21] ||
                    calculatedChecksumBytes[2] != rawData[offset + 22] ||
                    calculatedChecksumBytes[3] != rawData[offset + 23])
                {
                    return null;
                }
                return (command, payload, networkIndicator, messageBytes);
            }
            return null;
        }

        internal static Message Deserialize(byte[] rawData, int offset = 0)
        {
            var tuple= DeserializeToTuple(rawData, offset);
            if(tuple.HasValue)
            {
                var (command, payload, networkIndicator, messageBytes) = tuple.Value;
                return new Message(command, payload, networkIndicator, messageBytes);
            }
            return null;  
        }

        public static T Deserialize<T>(byte[] rawData, int offset = 0) where T : Message
        {
            var commandBytes = new char[12];
            Array.Copy(rawData, 4, commandBytes, 0, 12);
            var command = (new string(commandBytes)).Replace("\0", "");

            switch (command)
            {
                case "version":
                    {
                        return VersionMessage.Deserialize(rawData, offset) as T;
                    }
                case "verack":
                    {
                        return VersionAcknowledgeMessage.Deserialize(rawData, offset) as T;
                    }
                case "addr":
                    {
                        return AddressMessage.Deserialize(rawData, offset) as T;
                    }
                case "inv":
                    {
                        return InventoryMessage.Deserialize(rawData, offset) as T;
                    }
                case "getdata":
                    {
                        return GetDataMessage.Deserialize(rawData, offset) as T;
                    }
                case "notfound":
                    {
                        return NotFoundMessage.Deserialize(rawData, offset) as T;
                    }
                case "getblocks":
                    {
                        return GetBlocksMessage.Deserialize(rawData, offset) as T;
                    }
                case "getheaders":
                    {
                        return GetHeadersMessage.Deserialize(rawData, offset) as T;
                    }
                case "tx":
                    {
                        return TransactionMessage.Deserialize(rawData, offset) as T;
                    }
                case "block":
                    {
                        return BlockMessage.Deserialize(rawData, offset) as T;
                    }
                case "headers":
                    {
                        return HeadersMessage.Deserialize(rawData, offset) as T;
                    }
                case "getaddr":
                    {
                        return GetAddressMessage.Deserialize(rawData, offset) as T;
                    }
                case "mempool":
                case "checkorder":
                case "submitorder":
                case "reply":
                case "ping":
                case "pong":
                case "reject":
                case "filterload":
                case "filteradd":
                case "filterclear":
                case "merkleblock":
                case "alert":
                case "sendheaders":
                case "feefilter":
                case "sendcmpct":
                case "cmpctblock":
                case "getblocktxn":
                case "blocktxn":                

                default:
                    {
                        return Message.Deserialize(rawData, offset) as T;
                    }

            }
        }

        public override int Serialize(byte[] buffer, int offset = 0)
        {
            Buffer.BlockCopy(this.messageBytes, 0, buffer, offset, this.messageBytes.Length);
            return this.messageBytes.Length;
        }
    }
}
