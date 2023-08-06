using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointAdapters.AdaptersInterfaces;

public interface IEndpointAdapter
{
    Task<ValueConclusion> ProcessAsync(HttpRequestMessage httpContent, ClaimsIdentity? claimsIdentity);
}