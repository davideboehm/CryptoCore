using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    internal class Message
    {
        public string Text { get; private set; }
        public Dictionary<string, string> KeyValuePairs { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MessageType MessageType { get; private set; }
        
        public Message(MessageType messageType, string message, Dictionary<string, string> keyValuePairs)
        {
            this.MessageType = messageType;
            this.Text = message;
            this.KeyValuePairs = keyValuePairs;
            this.Timestamp = DateTime.UtcNow;
        }
    }
}
