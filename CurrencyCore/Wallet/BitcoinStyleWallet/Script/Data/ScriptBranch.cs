using CurrencyCore.Wallet.BitcoinStyleWallet.Script.Commands;
namespace CurrencyCore.Wallet.BitcoinStyleWallet.Script.Data
{
    /// <summary>
    /// Represents a script program that is a branch of an if/notif command
    /// </summary>
    public class ScriptBranch : ScriptData
    {
        private ScriptProgram program;
        protected override byte[] ByteValue { get => this.program.GetBytes(); }
        public ScriptBranch(ScriptProgram program) : base(null)
        {
            this.program = program;
        }

        public ScriptResult Execute(ScriptProgramStack currentState)
        {
            program.Evaluate(currentState);
            return ScriptResult.Success;
        }
        public override int Size
        {
            get
            {
                return this.program.Size;
            }
        }
    }
}
