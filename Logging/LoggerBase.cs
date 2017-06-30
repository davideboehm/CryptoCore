using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public abstract class LoggerBase : ILogger
    {
        public void LogMessage(string message, Dictionary<string, string> keyValuePairs = null)
        {
            this.Write(new Message(MessageType.Info, message, keyValuePairs));
        }

        public void LogError(string message, Dictionary<string, string> keyValuePairs = null)
        {
            this.Write(new Message(MessageType.Error, message, keyValuePairs));
        }

        public void LogWarning(string message, Dictionary<string, string> keyValuePairs = null)
        {
            this.Write(new Message(MessageType.Warning, message, keyValuePairs));
        }
        
        internal abstract void Write(Message message);
    }
}
