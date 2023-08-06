using Auth.Infrastructure.AdlemanProtocol.AdlemanInterfaces;

namespace Auth.Infrastructure.AdlemanProtocol.AdlemanAbstracts;

public abstract class AdlemanAbstract : IAdleman
{
    public abstract Task<string> EncryptAsync(string plainText, string publicKey);
    public abstract Task<string> DecryptAsync(string cipherText, string privateKey);

    public abstract Task<string> SignAsync(string plainText, string privateKey);
    public abstract Task<bool> VerifyAsync(string plainText, string signature, string publicKey);
    public abstract Task<(string publicKey, string privateKey)> CreateKeyPairAsync(int keySize);
}