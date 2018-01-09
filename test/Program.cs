using CurrencyCore.Coin;
using Exchanges;
using ExchangesCore;
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
            var test = new CurrencyAmount(1, CurrencyType.Bitcoin);
            var test2 = new CurrencyAmount(1, CurrencyType.Bitcoin);
            var test3 = CurrencyAmount.Zero;

            var sum = test + test3;
            sum = sum + test2;
        }
    }
}
