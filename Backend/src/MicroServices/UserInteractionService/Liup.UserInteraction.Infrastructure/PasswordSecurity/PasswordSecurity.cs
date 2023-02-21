using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace Liup.UserInteraction.Infrastructure.PasswordSecurity;

enum ArgonConfig
{
    Type = Isopoh.Cryptography.Argon2.Argon2Type.DataIndependentAddressing,
    Version = Argon2Version.Nineteen,
    TimeCost = 20,
    MemoryCost = 1024,
    Lanes = 1,
    Threads = 1,
    HashLength = 75
}

public static class PasswordSecurity
{
    private static readonly RandomNumberGenerator Rng = System.Security.Cryptography.RandomNumberGenerator.Create();

    public static async Task<string> HashPassword(string? password)
    {
        if (password is null)
            return "Password cannot be null";

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] salt = new byte[16];
        Rng.GetBytes(salt);

        var config = new Argon2Config
        {
            Type = (Isopoh.Cryptography.Argon2.Argon2Type)ArgonConfig.Type,
            Version = (Argon2Version)ArgonConfig.Version,
            TimeCost = (int)ArgonConfig.TimeCost,
            MemoryCost = (int)ArgonConfig.MemoryCost,
            Lanes = (int)ArgonConfig.Lanes,
            Threads = (int)ArgonConfig.Threads,
            Password = passwordBytes,
            Salt = salt,
            HashLength = (int)ArgonConfig.HashLength
        };

        var argon2 = new Isopoh.Cryptography.Argon2.Argon2(config);
        string hashString;
        using (SecureArray<byte> secureArray = argon2.Hash())
        {
            hashString = config.EncodeString(secureArray.Buffer);
        }

        return await Task.FromResult(hashString);
    }

    public static Task<bool> VerifyPassword(string? password, string hashString)
    {
        if (password is null || hashString is null)
            return Task.FromResult(false);

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        var config = new Argon2Config
        {
            Type = (Isopoh.Cryptography.Argon2.Argon2Type)ArgonConfig.Type,
            Version = (Argon2Version)ArgonConfig.Version,
            TimeCost = (int)ArgonConfig.TimeCost,
            MemoryCost = (int)ArgonConfig.MemoryCost,
            Lanes = (int)ArgonConfig.Lanes,
            Threads = (int)ArgonConfig.Threads,
            Password = passwordBytes,
            HashLength = (int)ArgonConfig.HashLength
        };

        var verifyConfig = new Isopoh.Cryptography.Argon2.Argon2(config);
        SecureArray<byte>? secureArray;
        if (config.DecodeString(hashString, out secureArray))
        {
            using (var hashToVerify = verifyConfig.Hash())
            {
                if (secureArray != null && hashToVerify != null)
                {
                    //return Task.FromResult(secureArray.Buffer.SequenceEqual(hashToVerify.Buffer));
                    return Task.FromResult(Isopoh.Cryptography.Argon2.Argon2.FixedTimeEquals(secureArray, hashToVerify));
                }
            }
        }

        return Task.FromResult(false);
    }
}
