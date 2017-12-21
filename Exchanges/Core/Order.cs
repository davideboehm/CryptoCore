namespace ExchangesCore
{
    using CurrencyCore.Coin;
    using System;
    
    public struct Order
    {
        public enum OrderType
        {
            Unknown,
            Buy,
            Sell
        };
                
        public Order(
            OrderType type,
            CurrencyType coinType,
            CurrencyType currencyType,
            Price price,
            CurrencyAmount quantity,
            CurrencyAmount total)
            : this()
        {
            this.StockType = coinType;
            this.CurrencyType = currencyType;
            this.Type = type;
            this.Quantity = quantity;
            this.Total = total;
            this.Price = price;
        }

        public CurrencyType CurrencyType { get; private set; }
        public CurrencyType StockType { get; private set; }
        public OrderType Type { get; private set; }
        public Price Price { get; private set; }
        public CurrencyAmount Quantity { get; private set; }
        public CurrencyAmount Total { get; private set; }

        public Order Clone()
        {
            return new Order(
                this.Type,
                this.StockType,
                this.CurrencyType,
                this.Price,
                this.Quantity,
                this.Total);
        }
        
        public override string ToString()
        {
            var result = Enum.GetName(typeof(OrderType), this.Type) + " " + this.Quantity + " "
                         + Enum.GetName(typeof(CurrencyType), this.StockType) + " coins for "
                         + Enum.GetName(typeof(CurrencyType), this.CurrencyType) + " at "
                         + this.Price + " " + Enum.GetName(typeof(CurrencyType), this.StockType)
                         + "s per " + Enum.GetName(typeof(CurrencyType), this.CurrencyType) + ".";
            return result;
        }
        
        public override int GetHashCode()
        {
            int hashCode = 1;

            hashCode = (hashCode * 1061) ^ this.Type.GetHashCode();
            hashCode = (hashCode * 1061) ^ this.Price.GetHashCode();
            hashCode = (hashCode * 1061) ^ this.StockType.GetHashCode();
            hashCode = (hashCode * 1061) ^ this.CurrencyType.GetHashCode();
            hashCode = (hashCode * 1061) ^ this.Quantity.GetHashCode();
            return hashCode;
        }
    }
}
