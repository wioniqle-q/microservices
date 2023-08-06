using Auth.Infrastructure.SanitizeProtocol.SanitizeInterfaces;

namespace Auth.Infrastructure.SanitizeProtocol.SanitizeAbstractions;

public abstract class SanitizeAbstract : ISanitize
{
    public abstract Task<string> SanitizeAsync(string input);
}