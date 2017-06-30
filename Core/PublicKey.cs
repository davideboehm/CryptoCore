namespace Core
{
    public abstract class PublicKey: Key
    {
        protected PublicKey(byte[] value)
            : base(value)
        {
        }
        protected PublicKey(string value)
            : base(value)
        {
        }
    }
}
