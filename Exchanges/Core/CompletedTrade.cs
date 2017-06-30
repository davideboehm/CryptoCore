using System;
using System.Collections.Generic;

namespace ExchangesCore
{
        using CurrencyCore.Coin;
    using ExchangesCore;

    public struct CompletedTrade
    {
        public readonly TradeType Type;
        public readonly CoinType StockCoin;
        public readonly CoinType CurrencyCoin;
        public readonly DateTime DateCompleted;
        public readonly CoinAmount Amount;
        public readonly Price Price;

        public CompletedTrade(
        TradeType type,
        CoinType stockCoin,
        CoinType currencyCoin,
        Price price,
        CoinAmount amount,
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
            var stockAbbreviation = CoinInfo.GetCoinInfo(this.StockCoin).GetAbbreviations()[0];
            var currencyAbbreviation = CoinInfo.GetCoinInfo(this.CurrencyCoin).GetAbbreviations()[0];

            return $"{verb} {Amount} {stockAbbreviation} at {this.Price} {currencyAbbreviation} per {stockAbbreviation} on {this.DateCompleted}";
        }

        public static implicit operator Trade(CompletedTrade completedTrade)
        {
            return new Trade(completedTrade.Type, completedTrade.StockCoin, completedTrade.CurrencyCoin);
        }
    }
}
