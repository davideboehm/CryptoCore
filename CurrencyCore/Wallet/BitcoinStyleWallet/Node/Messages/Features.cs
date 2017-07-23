using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Node.Messages
{
    public enum Features
    {
        NODE_NETWORK = 1,
        NODE_GETUTXO = 2,
        NODE_BLOOM = 4,
    }
}
