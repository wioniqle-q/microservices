using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;

namespace Auth.Infrastructure.ConcealmentProtocol.ConcealmentAbstracts;

public abstract class ConcealmentAbstract : IConcealment
{
    public abstract ValueTask<string> ConcealAsync(string plainText, string? phrase, byte[]? saltBytes);
    public abstract ValueTask<string> RevealAsync(string cipherText, string? phrase, byte[]? saltBytes);
}