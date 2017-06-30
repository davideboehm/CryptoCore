using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public interface ILogger
    {
        void LogMessage(string message, Dictionary<string, string> keyValuePairs = null);
        void LogError(string message, Dictionary<string, string> keyValuePairs = null);
        void LogWarning(string message, Dictionary<string, string> keyValuePairs = null);
    }
}
