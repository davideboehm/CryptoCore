namespace CurrencyCore.Wallet
{
    using CurrencyCore.Address;
    using CurrencyCore.Coin;
    using Newtonsoft.Json.Linq;

    public enum TransactionCategory
    {
        Generate,
        Receive
    }

    public class TransactionRecord
    {
        public AddressAlias Alias;
        public PublicAddress Address;
        public TransactionCategory TransactionCategory;
        public CoinAmount Amount;
        public int Confirmations;
        public bool IsGenerated;
        public string BlockHash;
        public int BlockIndex;
        public long BlockTime;
        public string TransactionId;
        public long Time;
        public long TimeReceived;

        private JToken jsonRepresentation;

        public TransactionRecord(JToken jToken)
        {
            this.jsonRepresentation = jToken;
        }
    }
}
