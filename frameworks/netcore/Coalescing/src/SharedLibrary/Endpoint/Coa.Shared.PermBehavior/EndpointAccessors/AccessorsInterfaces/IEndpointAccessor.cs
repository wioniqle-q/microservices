using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointAccessors.AccessorsInterfaces;

public interface IEndpointAccessor
{
    Task<ValueConclusion> VerifyAccessBehavior(string endpointToken, ClaimsIdentity? claimsIdentity);
}