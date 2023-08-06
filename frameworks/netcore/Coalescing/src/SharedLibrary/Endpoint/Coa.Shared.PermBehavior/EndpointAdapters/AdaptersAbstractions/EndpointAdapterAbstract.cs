using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointAdapters.AdaptersInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointAdapters.AdaptersAbstractions;

public abstract class EndpointAdapterAbstract : IEndpointAdapter
{
    public abstract Task<ValueConclusion> ProcessAsync(HttpRequestMessage httpContent, ClaimsIdentity? claimsIdentity);
}