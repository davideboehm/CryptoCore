namespace Core
{
    using System.Text;

    public abstract class Key
    {
        public byte[] Value;

        protected Key()
        {
        }

        protected Key(byte[] value)
        {
            this.Value = value;
        }

        protected Key(string value)
        {
            this.Value = Encoding.ASCII.GetBytes(value);
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(this.Value);
        }
    }
}
