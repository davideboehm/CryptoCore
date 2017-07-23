using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data;

namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands.PushCommands
{
    public abstract class PushConstant : PushData
    {
        private readonly ScriptData ValueToPush;
        protected PushConstant(byte opCode, ScriptData value): base (opCode, ScriptData.Nothing)
        {
            this.ValueToPush = value;
        }
        public override ScriptResult Execute(ScriptProgramStack currentState)
        {
            currentState.Push(this.ValueToPush);
            return ScriptResult.Success;
        }
    }

    public class PushFalse : PushConstant
    {
        public PushFalse() : base(0, BoolData.False)
        {
        }
    }
    public class PushNegativeOne : PushConstant
    {
        public PushNegativeOne() : base(79, new Int(-1))
        {
        }
    }
    public class PushTrue : PushConstant
    {
        public PushTrue() : base(81, new Data.Byte(1))
        {
        }
    }
    public class Push2 : PushConstant
    {
        public Push2() : base(82, new Int(2))
        {
        }
    }
    public class Push3 : PushConstant
    {
        public Push3() : base(83, new Int(3))
        {
        }
    }
    public class Push4 : PushConstant
    {
        public Push4() : base(84, new Int(4))
        {
        }
    }
    public class Push5 : PushConstant
    {
        public Push5() : base(85, new Int(5))
        {
        }
    }
    public class Push6 : PushConstant
    {
        public Push6() : base(86, new Int(6))
        {
        }
    }
    public class Push7 : PushConstant
    {
        public Push7() : base(87, new Int(7))
        {
        }
    }
    public class Push8 : PushConstant
    {
        public Push8() : base(88, new Int(8))
        {
        }
    }
    public class Push9 : PushConstant
    {
        public Push9() : base(89, new Int(9))
        {
        }
    }
    public class Push10 : PushConstant
    {
        public Push10() : base(90, new Int(10))
        {
        }
    }
    public class Push11 : PushConstant
    {
        public Push11() : base(91, new Int(11))
        {
        }
    }
    public class Push12 : PushConstant
    {
        public Push12() : base(92, new Int(12))
        {
        }
    }
    public class Push13 : PushConstant
    {
        public Push13() : base(93, new Int(13))
        {
        }
    }
    public class Push14 : PushConstant
    {
        public Push14() : base(94, new Int(14))
        {
        }
    }
    public class Push15 : PushConstant
    {
        public Push15() : base(95, new Int(15))
        {
        }
    }
    public class Push16 : PushConstant
    {
        public Push16() : base(96, new Int(16))
        {
        }
    }
}
