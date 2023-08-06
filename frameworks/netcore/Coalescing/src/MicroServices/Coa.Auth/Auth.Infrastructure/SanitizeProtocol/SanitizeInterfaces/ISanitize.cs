namespace Auth.Infrastructure.SanitizeProtocol.SanitizeInterfaces;

public interface ISanitize
{
    Task<string> SanitizeAsync(string input);
}