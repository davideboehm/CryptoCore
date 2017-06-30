namespace CurrencyCore.Wallet
{
    using CurrencyCore.Address;
    using System.Collections.Generic;

    public class AddressAlias
    {
        public static AddressAlias All = new AddressAlias("*");
        public string Alias;

        public List<PublicAddress> KnownAssociatedAddresses;

        public AddressAlias(string alias)
        {
            this.Alias = alias;
            this.KnownAssociatedAddresses = new List<PublicAddress>();
        }

        public override string ToString()
        {
            return this.Alias;
        }
    }
}
