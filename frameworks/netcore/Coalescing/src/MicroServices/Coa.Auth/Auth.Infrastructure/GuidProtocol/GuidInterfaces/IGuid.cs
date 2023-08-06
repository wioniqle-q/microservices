namespace Auth.Infrastructure.GuidProtocol.GuidInterfaces;

public interface IGuid
{
    Task<Guid> GenerateGuidAsync();
}