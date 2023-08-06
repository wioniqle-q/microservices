using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using Auth.Infrastructure.AdlemanProtocol.AdlemanAbstracts;

namespace Auth.Infrastructure.AdlemanProtocol;

public sealed class Adleman : AdlemanAbstract
{
    public override Task<string> EncryptAsync(string plainText, string publicKey)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKey);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);

        var base64Array = ArrayPool<byte>.Shared.Rent(PoolCalculate(cipherBytes.Length));

        var base64 = Convert.ToBase64String(new Span<byte>(cipherBytes));

        var base64Length = Encoding.ASCII.GetBytes(base64, base64Array);
        var base64String = Encoding.ASCII.GetString(base64Array, 0, base64Length);

        ArrayPool<byte>.Shared.Return(base64Array);
        Array.Clear(base64Array, 0, base64Array.Length);
        Array.Clear(cipherBytes, 0, cipherBytes.Length);
        Array.Clear(plainBytes, 0, plainBytes.Length);

        return Task.FromResult(base64String);
    }

    public override Task<string> DecryptAsync(string cipherText, string privateKey)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKey);

        var base64Array = ArrayPool<byte>.Shared.Rent(cipherText.Length);
        var base64Length = Encoding.ASCII.GetBytes(cipherText, base64Array);

        var encryptedData = Convert.FromBase64String(Encoding.ASCII.GetString(base64Array, 0, base64Length));
        ArrayPool<byte>.Shared.Return(base64Array);

        var decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
        var decryptedText = Encoding.UTF8.GetString(decryptedData);

        Array.Clear(encryptedData, 0, encryptedData.Length);
        Array.Clear(base64Array, 0, base64Array.Length);
        Array.Clear(decryptedData, 0, decryptedData.Length);

        return Task.FromResult(decryptedText);
    }

    public override Task<string> SignAsync(string plainText, string privateKey)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKey);

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var signatureBytes = rsa.SignData(plainBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        var base64Array = ArrayPool<byte>.Shared.Rent(PoolCalculate(signatureBytes.Length));
        var base64 = Convert.ToBase64String(new Span<byte>(signatureBytes));

        var base64Length = Encoding.ASCII.GetBytes(base64, base64Array);
        var base64String = Encoding.ASCII.GetString(base64Array, 0, base64Length);

        ArrayPool<byte>.Shared.Return(base64Array);
        Array.Clear(base64Array, 0, base64Array.Length);
        Array.Clear(signatureBytes, 0, signatureBytes.Length);
        Array.Clear(plainBytes, 0, plainBytes.Length);

        return Task.FromResult(base64String);
    }

    public override Task<bool> VerifyAsync(string plainText, string signature, string publicKey)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKey);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var base64Array = ArrayPool<byte>.Shared.Rent(signature.Length);
        var base64Length = Encoding.ASCII.GetBytes(signature, base64Array);

        var signatureBytes = Convert.FromBase64String(Encoding.ASCII.GetString(base64Array, 0, base64Length));
        ArrayPool<byte>.Shared.Return(base64Array);

        var isVerified =
            rsa.VerifyData(plainBytes, signatureBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        Array.Clear(signatureBytes, 0, signatureBytes.Length);
        Array.Clear(base64Array, 0, base64Array.Length);
        Array.Clear(plainBytes, 0, plainBytes.Length);

        return Task.FromResult(isVerified);
    }

    public override Task<(string publicKey, string privateKey)> CreateKeyPairAsync(int keySize)
    {
        using var rsa = RSA.Create(keySize);
        var publicKey = rsa.ToXmlString(false);
        var privateKey = rsa.ToXmlString(true);
        return Task.FromResult((publicKey, privateKey));
    }

    private static int PoolCalculate(int length)
    {
        return length is not 0 ? 4 * ((length + 2) / 3) : 0;
    }
}