using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Infrastructure.PasswordObfuscation;

public static class ObfuscatePassword
{
    private const string RustDllPath = "hello_world.dll";

    private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();
    private static readonly byte[] Salt = new byte[32];

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr create_hash(IntPtr password, IntPtr salt, uint iterations, uint memory,
        uint parallelism, uint hashLength);

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool verify_hash(IntPtr hash, IntPtr password);

    [DllImport(RustDllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_hash(IntPtr hash);

    public static async Task<string> Obfuscate(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "Password cannot be null";

        Generator.GetBytes(Salt);

        var hashPtr = CreateHash(password, Encoding.UTF8.GetString(Salt), (uint)ObfuscateEnum.TimeCost,
            (uint)ObfuscateEnum.MemoryCost, (uint)ObfuscateEnum.Lanes, (uint)ObfuscateEnum.HashLength);

        var hash = Marshal.PtrToStringAnsi(hashPtr)!;
        free_hash(hashPtr);

        return await Task.FromResult(hash);
    }

    public static Task<bool> Verify(string obfuscatedPassword, string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(obfuscatedPassword))
            return Task.FromResult(false);

        var hashPtr = Marshal.StringToHGlobalAnsi(obfuscatedPassword);

        var isValid = VerifyHash(hashPtr, password);
        Marshal.FreeHGlobal(hashPtr);

        return Task.FromResult(isValid);
    }

    private static IntPtr CreateHash(string password, string salt, uint iterations, uint memory, uint parallelism,
        uint hashLength)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt) || iterations == 0 || memory == 0 ||
            parallelism == 0 || hashLength == 0)
            return IntPtr.Zero;

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var passwordPtr = Marshal.StringToHGlobalAnsi(password);

        var saltPtr = Marshal.AllocHGlobal(saltBytes.Length);
        Marshal.Copy(saltBytes, 0, saltPtr, saltBytes.Length);

        var hashPtr = create_hash(passwordPtr, saltPtr, iterations, memory, parallelism, hashLength);
        Marshal.FreeHGlobal(saltPtr);

        return hashPtr;
    }

    private static bool VerifyHash(IntPtr hash, string password)
    {
        if (string.IsNullOrEmpty(password) || hash == IntPtr.Zero)
            return false;

        var passwordPtr = Marshal.StringToHGlobalAnsi(password);
        var isValid = verify_hash(hash, passwordPtr);

        Marshal.FreeHGlobal(passwordPtr);
        return isValid;
    }
}