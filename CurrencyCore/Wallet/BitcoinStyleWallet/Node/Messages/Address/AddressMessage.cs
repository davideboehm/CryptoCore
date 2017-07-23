using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CurrencyCore.Wallet.BitcoinStyleWallet.SerializableExtensions;
namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    /// <summary>
    /// Class represents an Address or "addr" message.  It stores a list of peer information.  For each peer it has the last time they were seen, the services they advertise, their ip address, and their port
    /// </summary>
    public class AddressMessage : Message
    {
        public readonly ReadOnlyCollection<PeerInfo> PeerList;
        public AddressMessage(List<PeerInfo> peerList) : base("addr", peerList.Serialize())
        {
            this.PeerList = peerList.AsReadOnly();
        }
        private AddressMessage(List<PeerInfo> peerList, byte[] payload, uint networkIndicator, byte[] messageBytes ) : base("addr", payload, networkIndicator, messageBytes)
        {
            this.PeerList = peerList.AsReadOnly();
        }
        
        private static byte[] Serialize(List<PeerInfo> peerList)
        {
            var sizeBytes = ((VInt)peerList.Count).Serialize();
            var result = new byte[sizeBytes.Length + peerList.Count * PeerInfo.Size];
            Buffer.BlockCopy(sizeBytes, 0, result, 0, sizeBytes.Length);
            int offset = sizeBytes.Length;
            foreach (var peer in peerList)
            {
                peer.Serialize(result, offset);
                offset += PeerInfo.Size;
            }
            return result;
        }

        public new static AddressMessage Deserialize(byte[] rawData, int offset)
        {
            var tuple = DeserializeToTuple(rawData, offset);
            if (!tuple.HasValue)
            {
                return null;
            }

            var (command, payload, networkIndicator, messageBytes) = tuple.Value;

            var peerList = new List<PeerInfo>();
            if (payload.Length > PeerInfo.Size)
            {
                var count = new VInt(payload.Length);
                if (count < 2000 && payload.Length / PeerInfo.Size == (long)count)
                {
                    var offsetIntoPayload = payload.Length % PeerInfo.Size;
                    for (var index = 0; index < (int)count; index++)
                    {

                        var peer = PeerInfo.Deserialize(payload, index * PeerInfo.Size + offsetIntoPayload);
                        if (peer.HasValue)
                        {
                            peerList.Add(peer.Value);
                        }
                    }

                    return new AddressMessage(peerList, payload, networkIndicator, messageBytes);
                }
            }
            return null;
        }
    }  
}
