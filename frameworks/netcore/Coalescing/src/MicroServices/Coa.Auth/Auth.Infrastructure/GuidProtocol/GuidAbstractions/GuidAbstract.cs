using Auth.Infrastructure.GuidProtocol.GuidInterfaces;

namespace Auth.Infrastructure.GuidProtocol.GuidAbstractions;

public abstract class GuidAbstract : IGuid
{
    public abstract Task<Guid> GenerateGuidAsync();
}