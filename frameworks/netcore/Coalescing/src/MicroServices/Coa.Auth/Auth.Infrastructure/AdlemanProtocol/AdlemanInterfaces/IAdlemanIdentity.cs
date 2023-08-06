namespace Auth.Infrastructure.AdlemanProtocol.AdlemanInterfaces;

public interface IAdlemanIdentity
{
    Task<string> EncryptAsync(string plainText, string publicKey);
    Task<string> DecryptAsync(string cipherText, string privateKey);

    Task<string> SignAsync(string plainText, string privateKey);
    Task<bool> VerifyAsync(string plainText, string signature, string publicKey);

    Task<(string publicKey, string privateKey)> CreateKeyPairAsync(int keySize);
}