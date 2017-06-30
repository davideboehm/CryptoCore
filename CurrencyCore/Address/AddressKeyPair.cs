
namespace CurrencyCore.Address
{
        using Core;

    public class AddressKeyPair : KeyPair<AddressPublicKey,AddressPrivateKey>
    {
        public AddressKeyPair(AddressPublicKey publicKey, AddressPrivateKey privateKey) : base(publicKey, privateKey)
        {

        }
    }
}
