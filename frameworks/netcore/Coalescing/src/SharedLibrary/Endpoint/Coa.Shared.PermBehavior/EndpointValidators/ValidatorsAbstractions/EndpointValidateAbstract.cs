using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;
using Coa.Shared.PermBehavior.EndpointValidators.ValidatorsInterfaces;

namespace Coa.Shared.PermBehavior.EndpointValidators.ValidatorsAbstractions;

public abstract class EndpointValidateAbstract : IEndpointValidate
{
    public abstract Task<ValueConclusion> ValidateAccessBehavior(HttpRequestMessage httpContent);
}