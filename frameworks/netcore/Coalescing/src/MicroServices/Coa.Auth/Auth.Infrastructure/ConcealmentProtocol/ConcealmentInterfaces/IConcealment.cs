namespace Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;

public interface IConcealment
{
    ValueTask<string> ConcealAsync(string plainText, string? phrase, byte[]? saltBytes);
    ValueTask<string> RevealAsync(string cipherText, string? phrase, byte[]? saltBytes);
}