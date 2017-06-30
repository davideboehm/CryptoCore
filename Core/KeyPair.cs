namespace Core
{
    public class KeyPair<TPublic, TPrivate>
    {
        public TPublic PublicKey { get; protected set; }
        public TPrivate PrivateKey { get; protected set; }
        public KeyPair(TPublic publicKey, TPrivate privateKey)
        {
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;
        }
    }

    public class KeyPair: KeyPair<PublicKey,PrivateKey>
    {
        public KeyPair(PublicKey publicKey, PrivateKey privateKey) : base(publicKey, privateKey)
        {
        }
    }
}
