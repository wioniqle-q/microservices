using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentAbstracts;

namespace Auth.Infrastructure.ConcealmentProtocol;

public sealed class BaseConcealment : ConcealmentAbstract
{
    private const string RustDllPath = "aes_dll.dll";

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr encrypt(IntPtr input, SizeT inputLen, IntPtr key, ref SizeT encryptedDataLen);

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr decrypt(IntPtr encryptedData, SizeT encryptedDataLen, IntPtr key,
        ref SizeT decryptedDataLen);

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_buffer(IntPtr ptr, SizeT size);

    public override async ValueTask<string> ConcealAsync(string plainText, string? phrase, byte[]? saltBytes)
    {
        if (string.IsNullOrEmpty(plainText))
            return await new ValueTask<string>(string.Empty);

        if (string.IsNullOrEmpty(phrase))
            phrase = ConcealmentOptions.DefaultPassPhrase;

        saltBytes ??= ConcealmentOptions.DefaultSalt;

        using var password = new Rfc2898DeriveBytes(phrase, saltBytes, 40 * 1000, HashAlgorithmName.SHA512);
        var keyBytes = password.GetBytes(ConcealmentOptions.Keysize / 8);

        var encryptedData = Encrypt(plainText, keyBytes);

        Array.Clear(keyBytes, 0, keyBytes.Length);
        Array.Clear(password.Salt, 0, password.Salt.Length);

        var base64Array = ArrayPool<byte>.Shared.Rent(4 * ((encryptedData.Length + 2) / 3));

        var base64 = Convert.ToBase64String(new Span<byte>(encryptedData));

        var base64Length = Encoding.ASCII.GetBytes(base64, base64Array);
        var base64String = Encoding.ASCII.GetString(base64Array, 0, base64Length);

        ArrayPool<byte>.Shared.Return(base64Array);

        Array.Clear(keyBytes, 0, keyBytes.Length);
        Array.Clear(encryptedData, 0, encryptedData.Length);
        Array.Clear(base64Array, 0, base64Array.Length);

        return await new ValueTask<string>(base64String);
    }

    public override async ValueTask<string> RevealAsync(string cipherText, string? phrase,
        byte[]? saltBytes)
    {
        if (string.IsNullOrEmpty(cipherText))
            return await new ValueTask<string>(string.Empty);

        if (string.IsNullOrEmpty(phrase))
            phrase = ConcealmentOptions.DefaultPassPhrase;

        saltBytes ??= ConcealmentOptions.DefaultSalt;

        using var password = new Rfc2898DeriveBytes(phrase, saltBytes, 40 * 1000, HashAlgorithmName.SHA512);
        var keyBytes = password.GetBytes(ConcealmentOptions.Keysize / 8);

        var base64Array = ArrayPool<byte>.Shared.Rent(cipherText.Length);
        var base64Length = Encoding.ASCII.GetBytes(cipherText, base64Array);

        var encryptedData = Convert.FromBase64String(Encoding.ASCII.GetString(base64Array, 0, base64Length));
        ArrayPool<byte>.Shared.Return(base64Array);

        var decryptedData = Decrypt(encryptedData, keyBytes);

        Array.Clear(keyBytes, 0, keyBytes.Length);
        Array.Clear(password.Salt, 0, password.Salt.Length);
        Array.Clear(encryptedData, 0, encryptedData.Length);
        Array.Clear(base64Array, 0, base64Array.Length);

        return await new ValueTask<string>(decryptedData);
    }

    private static byte[] Encrypt(string input, byte[] key)
    {
        if (string.IsNullOrEmpty(input) || key.Length == 0)
            return Array.Empty<byte>();

        var inputPtr = Marshal.StringToHGlobalAnsi(input);
        var inputLen = new SizeT { Value = (ulong)input.Length };

        var keyPtr = Marshal.AllocHGlobal(key.Length);
        Marshal.Copy(key, 0, keyPtr, key.Length);

        var encryptedDataPtr = encrypt(inputPtr, inputLen, keyPtr, ref inputLen);

        var encryptedData = new byte[inputLen.Value];
        Marshal.Copy(encryptedDataPtr, encryptedData, 0, (int)inputLen.Value);

        free_buffer(encryptedDataPtr, inputLen);

        Marshal.FreeHGlobal(keyPtr);
        Marshal.FreeHGlobal(inputPtr);

        Array.Clear(key, 0, key.Length);

        return encryptedData;
    }

    private static string Decrypt(byte[] encryptedData, byte[] key)
    {
        if (encryptedData.Length == 0 || key.Length == 0)
            return string.Empty;

        var encryptedDataPtr = Marshal.AllocHGlobal(encryptedData.Length);
        Marshal.Copy(encryptedData, 0, encryptedDataPtr, encryptedData.Length);
        var encryptedDataLen = new SizeT { Value = (ulong)encryptedData.Length };

        var keyPtr = Marshal.AllocHGlobal(key.Length);
        Marshal.Copy(key, 0, keyPtr, key.Length);

        var decryptedDataPtr = decrypt(encryptedDataPtr, encryptedDataLen, keyPtr, ref encryptedDataLen);

        var decryptedData = new byte[encryptedDataLen.Value];
        Marshal.Copy(decryptedDataPtr, decryptedData, 0, (int)encryptedDataLen.Value);

        free_buffer(decryptedDataPtr, encryptedDataLen);

        Marshal.FreeHGlobal(keyPtr);
        Marshal.FreeHGlobal(encryptedDataPtr);

        Array.Clear(key, 0, key.Length);

        return Encoding.Default.GetString(decryptedData);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SizeT
    {
        public ulong Value;
    }
}