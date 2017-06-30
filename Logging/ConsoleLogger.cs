using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class ConsoleLogger: LoggerBase
    {     
        internal override void Write(Message message)
        {

            var output =   $"[{message.Timestamp}][{message.MessageType}] : {message.Text}";
            if (message.KeyValuePairs != null)
            {
                var paramsList = message.KeyValuePairs.Select(kvp => $"({kvp.Key}:{kvp.Value})");
                output += " {" + string.Join(", ", paramsList) + "}";
            }
            Console.WriteLine(output);
        }
    }
}
