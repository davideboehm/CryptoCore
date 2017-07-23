using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public class InventoryMessage:Message
    {
        public InventoryMessage():base("inv")
        {

        }
    }
}
