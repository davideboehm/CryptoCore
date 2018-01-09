namespace CurrencyCore.Coin
{
    using Core.Functional;
    using System;

    public partial class  CurrencyAmount : TypedNumeric<CurrencyUnit, CurrencyAmount>
    {
        public CurrencyAmount GetTypedZero(CurrencyType type)
        {
            return new CurrencyAmount(0, type);
        }
 
        public static readonly CurrencyAmount Zero = new CurrencyAmount(0, CurrencyUnit.Unit);

        public readonly CurrencyType Currency;
        public CurrencyAmount(Numeric value, CurrencyType currency) : base(value, new CurrencyUnit(currency))
        {
            this.Currency = currency;
        }

        protected CurrencyAmount(Numeric value, CurrencyUnit unit) : base(value,unit)
        {
        }     
        
        public override string ToString()
        {
            return string.Format("{0:F8}", (decimal)this.Value) + " " + this.Units.ToString();            
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }    
}
