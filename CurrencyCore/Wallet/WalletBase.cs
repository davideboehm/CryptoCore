using CurrencyCore.Address;
using CurrencyCore.Coin;
using System.Collections.Generic;

namespace CurrencyCore.Wallet
{
    public abstract class WalletBase : IWallet
    {
        public abstract List<PublicAddress> GetAddress(AddressAlias alias);
        public abstract CoinAmount GetAliasBalanceStartingFromTime(AddressAlias alias, long earliestTimeThatCounts);
        public abstract List<AddressAlias> GetAliasList();
        public abstract CoinAmount GetBalance(AddressAlias alias);
        public abstract int GetBlockCount();
        public abstract int GetTransactionTimeOfLastTransaction(AddressAlias alias);
        public abstract void ImportPrivateKey(AddressPrivateKey privateKey);
        public abstract bool IsMine(PublicAddress address);
        public abstract bool IsValidAddress(PublicAddress address);
        public abstract bool IsValidAddress(string address);
        public abstract List<TransactionRecord> ListTransactions(AddressAlias alias, int count = 10, int skip = 0);
        public abstract void SendFrom(AddressAlias from, PublicAddress destinationAddress, CoinAmount amount);
        public abstract void SendMany(AddressAlias from, List<(PublicAddress, CoinAmount)> sendTuples);
        public abstract void SendToAddress(PublicAddress destinationAddress, CoinAmount amount);
    }
}
