namespace CurrencyCore.Wallet
{
    using CurrencyCore.Address;
    using CurrencyCore.Coin;
    using System.Collections.Generic;

    public interface IWallet
    {
        List<PublicAddress> GetAddress(AddressAlias alias);
        CoinAmount GetAliasBalanceStartingFromTime(AddressAlias alias, long earliestTimeThatCounts);
        List<AddressAlias> GetAliasList();
        CoinAmount GetBalance(AddressAlias alias);
        int GetBlockCount();
        int GetTransactionTimeOfLastTransaction(AddressAlias alias);
        void ImportPrivateKey(AddressPrivateKey privateKey);
        bool IsMine(PublicAddress address);
        bool IsValidAddress(PublicAddress address);
        bool IsValidAddress(string address);
        List<TransactionRecord> ListTransactions(AddressAlias alias, int count = 10, int skip = 0);
        void SendFrom(AddressAlias from, PublicAddress destinationAddress, CoinAmount amount);
        void SendMany(AddressAlias from, List<(PublicAddress, CoinAmount)> sendTuples);
        void SendToAddress(PublicAddress destinationAddress, CoinAmount amount);
    }
}
