using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointAccessors.AccessorsInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointAccessors.AccessorsAbstractions;

public abstract class EndpointAccessorAbstract : IEndpointAccessor
{
    public abstract Task<ValueConclusion> VerifyAccessBehavior(string endpointToken, ClaimsIdentity? claimsIdentity);
}