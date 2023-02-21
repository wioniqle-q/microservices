
namespace Liup.Authorization.Infrastructure.GlobalEncryption.Interfaces;

public interface IEncryptionProvider
{
    ValueTask<string> EncryptAsync(string? plainText, string? passPhrase, byte[]? saltBytes, CancellationToken cancellationToken);
    ValueTask<string> DecryptAsync(string cipherText, string? passPhrase, byte[]? saltBytes, CancellationToken cancellationToken);
}
