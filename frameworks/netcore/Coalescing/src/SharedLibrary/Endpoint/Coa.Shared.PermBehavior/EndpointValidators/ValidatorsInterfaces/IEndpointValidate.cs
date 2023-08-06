using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointValidators.ValidatorsInterfaces;

public interface IEndpointValidate
{
    Task<ValueConclusion> ValidateAccessBehavior(HttpRequestMessage httpContent);
}