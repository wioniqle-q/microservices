using System.Security.Cryptography;
using System.Text;
using Liup.Authorization.Infrastructure.GlobalEncryption.Interfaces;
using Microsoft.Extensions.Options;

namespace Liup.Authorization.Infrastructure.GlobalEncryption.Encryption;

public class EncryptionProvider : IEncryptionProvider
{
    private EncryptionOptions _encryptionOptions { get; set; }

    public EncryptionProvider(IOptions<EncryptionOptions> encryptionOptions)
    {
        _encryptionOptions = encryptionOptions.Value;
    }

    public async virtual ValueTask<string> EncryptAsync(string? plainText, string? passPhrase, byte[]? saltBytes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        if (string.IsNullOrWhiteSpace(passPhrase))
        {
            passPhrase = _encryptionOptions.PassPhrase;
        }

        if (saltBytes == null)
        {
            saltBytes = _encryptionOptions.SaltBytes;
        }

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes;

        using (var password = new Rfc2898DeriveBytes(passPhrase, saltBytes, _encryptionOptions.Iterations, HashAlgorithmName.SHA256))
        {
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.BlockSize = _encryptionOptions.KeySize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;

                using (var encryptor = symmetricKey.CreateEncryptor(password.GetBytes(_encryptionOptions.KeySize / 8), password.GetBytes(_encryptionOptions.InitVectorSize)))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            await cryptoStream.WriteAsync(plainBytes, 0, plainBytes.Length, cancellationToken);
                            await cryptoStream.FlushFinalBlockAsync(cancellationToken);
                            cipherBytes = memoryStream.ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                        }
                    }
                }

                symmetricKey.Clear();
            }
        }

        return Convert.ToBase64String(cipherBytes);
    }

    public async virtual ValueTask<string> DecryptAsync(string cipherText, string? passPhrase, byte[]? saltBytes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            throw new ArgumentNullException(nameof(cipherText));
        }

        if (string.IsNullOrWhiteSpace(passPhrase))
        {
            passPhrase = _encryptionOptions.PassPhrase;
        }

        if (saltBytes == null)
        {
            saltBytes = _encryptionOptions.SaltBytes;
        }

        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] plainBytes;

        using (var password = new Rfc2898DeriveBytes(passPhrase, saltBytes, _encryptionOptions.Iterations, HashAlgorithmName.SHA256))
        {
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.BlockSize = _encryptionOptions.KeySize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;

                using (var decryptor = symmetricKey.CreateDecryptor(password.GetBytes(_encryptionOptions.KeySize / 8), password.GetBytes(_encryptionOptions.InitVectorSize)))
                {
                    using (var memoryStream = new MemoryStream(cipherBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            plainBytes = new byte[cipherBytes.Length];
                            int decryptedByteCount = await cryptoStream.ReadAsync(plainBytes, 0, plainBytes.Length, cancellationToken);
                            memoryStream.Close();
                            cryptoStream.Close();
                            Array.Resize(ref plainBytes, decryptedByteCount);
                        }
                    }
                }

                symmetricKey.Clear();
            }
        }

        return Encoding.UTF8.GetString(plainBytes);
    }
}
