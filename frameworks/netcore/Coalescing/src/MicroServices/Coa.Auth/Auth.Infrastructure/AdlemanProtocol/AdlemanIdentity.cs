using Auth.Infrastructure.AdlemanProtocol.AdlemanAbstracts;

namespace Auth.Infrastructure.AdlemanProtocol;

public sealed class AdlemanIdentity : AdlemanIdentityAbstract
{
    private readonly Adleman _adleman;

    public AdlemanIdentity(Adleman adleman)
    {
        _adleman = adleman;
    }

    public override async Task<string> EncryptAsync(string plainText, string publicKey)
    {
        return await _adleman.EncryptAsync(plainText, publicKey);
    }

    public override async Task<string> DecryptAsync(string cipherText, string privateKey)
    {
        return await _adleman.DecryptAsync(cipherText, privateKey);
    }

    public override async Task<string> SignAsync(string plainText, string privateKey)
    {
        return await _adleman.SignAsync(plainText, privateKey);
    }

    public override async Task<bool> VerifyAsync(string plainText, string signature, string publicKey)
    {
        return await _adleman.VerifyAsync(plainText, signature, publicKey);
    }

    public override async Task<(string publicKey, string privateKey)> CreateKeyPairAsync(int keySize)
    {
        return await _adleman.CreateKeyPairAsync(keySize);
    }
}