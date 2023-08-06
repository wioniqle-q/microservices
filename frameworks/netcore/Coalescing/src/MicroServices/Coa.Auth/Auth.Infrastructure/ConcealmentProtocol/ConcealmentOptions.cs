namespace Auth.Infrastructure.ConcealmentProtocol;

public abstract class ConcealmentOptions
{
    public static int Keysize => 256;
    public static string DefaultPassPhrase => "1234567890123456";
    public static byte[] DefaultSalt { get; } = "1234567890123456"u8.ToArray();
}