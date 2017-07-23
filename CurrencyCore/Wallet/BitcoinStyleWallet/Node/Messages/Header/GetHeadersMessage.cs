using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public class GetHeadersMessage : Message
    {
        public GetHeadersMessage():base("getheader")
        {

        }
    }
}
