using Exchanges;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static Gemini gemini = new Gemini(new ConsoleLogger(), null);
        static void Main(string[] args)
        {
            var test = gemini.GetMarkets().Result;
        }
    }
}
