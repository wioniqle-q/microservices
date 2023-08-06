using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointAccessors.AccessorsInterfaces;
using Coa.Shared.PermBehavior.EndpointAdapters.AdaptersAbstractions;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;
using Coa.Shared.PermBehavior.EndpointValidators.ValidatorsInterfaces;

namespace Coa.Shared.PermBehavior.EndpointAdapters.Adapters;

public sealed class InitPermAdapter : EndpointAdapterAbstract
{
    private readonly IEndpointAccessor _endpointAccessor;
    private readonly IEndpointValidate _endpointValidate;

    public InitPermAdapter(IEndpointAccessor endpointAccessor, IEndpointValidate endpointValidate)
    {
        _endpointAccessor = endpointAccessor;
        _endpointValidate = endpointValidate;
    }

    public override async Task<ValueConclusion> ProcessAsync(HttpRequestMessage? httpContent,
        ClaimsIdentity? claimsIdentity)
    {
        if (httpContent is null)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "HttpContext is null.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "HttpContext is null.",
                ExceptionMessage = "HttpContext is null."
            };

        var endpointValidateResult = await _endpointValidate.ValidateAccessBehavior(httpContent).ConfigureAwait(false);
        if (endpointValidateResult.UniqueStatusCode is 1)
            return endpointValidateResult;

        var endpointAccessorResult = await _endpointAccessor
            .VerifyAccessBehavior(endpointValidateResult.Token!, claimsIdentity).ConfigureAwait(false);
        return endpointAccessorResult;
    }
}