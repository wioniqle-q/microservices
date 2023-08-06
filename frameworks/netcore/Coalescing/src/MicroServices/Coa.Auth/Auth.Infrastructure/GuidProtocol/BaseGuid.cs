using System.Security.Cryptography;
using Auth.Infrastructure.GuidProtocol.GuidAbstractions;

namespace Auth.Infrastructure.GuidProtocol;

public sealed class BaseGuid : GuidAbstract
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public override async Task<Guid> GenerateGuidAsync()
    {
        var guidBytes = new byte[16];
        _rng.GetBytes(guidBytes);

        return await Task.FromResult(new Guid(guidBytes));
    }
}