namespace Core
{
    public abstract class PrivateKey : Key
    {
        protected PrivateKey()
        {
        }

        protected PrivateKey(byte[] value)
            : base(value)
        {
        }

        protected PrivateKey(string value)
            : base(value)
        {
        }
    }
}
