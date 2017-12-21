using System;
using System.Collections.Generic;

namespace ExchangesCore
{
        using CurrencyCore.Coin;
    using ExchangesCore;

    public struct CompletedTrade
    {
        public readonly TradeType Type;
        public readonly CurrencyType StockCoin;
        public readonly CurrencyType CurrencyCoin;
        public readonly DateTime DateCompleted;
        public readonly CurrencyAmount Amount;
        public readonly Price Price;

        public CompletedTrade(
        TradeType type,
        CurrencyType stockCoin,
        CurrencyType currencyCoin,
        Price price,
        CurrencyAmount amount,
        DateTime dateCompleted)
        {
            this.Type = type;
            this.StockCoin = stockCoin;
            this.CurrencyCoin = currencyCoin;
            this.Price = price;
            this.Amount = amount;
            this.DateCompleted = dateCompleted;
        }
        public override string ToString()
        {
            var verb = this.Type == TradeType.Buy ? "Bought" : "Sold";
            var stockAbbreviation = Currency.GetDefaultAbbreviation(this.StockCoin);
            var currencyAbbreviation = Currency.GetDefaultAbbreviation(this.CurrencyCoin);

            return $"{verb} {Amount} {stockAbbreviation} for {this.Price} {currencyAbbreviation} per {stockAbbreviation} on {this.DateCompleted}";
        }

        public static implicit operator Trade(CompletedTrade completedTrade)
        {
            return new Trade(completedTrade.Type, completedTrade.StockCoin, completedTrade.CurrencyCoin);
        }
    }
}
