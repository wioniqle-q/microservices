
namespace Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory.InvestigationDisclosure;

public abstract class InvestigationAssesmentResult
{
    public virtual string? InvestigationResponse { get; set; }
}

public sealed class InvestigationResult : InvestigationAssesmentResult
{
    public InvestigationResult(string @InvestigationResult)
    {
        InvestigationResponse = @InvestigationResult;
    }

    public sealed override string? InvestigationResponse
    {
        get => _InvestigationResponse;
        set => _InvestigationResponse = value;
    }
    public volatile string? _InvestigationResponse;
}


