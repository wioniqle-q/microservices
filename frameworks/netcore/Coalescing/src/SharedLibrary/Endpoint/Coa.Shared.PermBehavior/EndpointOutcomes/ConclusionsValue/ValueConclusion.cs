using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsAbstractions;

namespace Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

public sealed class ValueConclusion : ConclusionsAbstract
{
    public ValueConclusion(int? uniqueStatusCode, string? description, string? uniqueInfo, string? exceptionId,
        string? exceptionType, string? exceptionMessage, string? token, string? refreshToken)
    {
        UniqueStatusCode = uniqueStatusCode;
        Description = description;
        UniqueInfo = uniqueInfo;
        ExceptionId = exceptionId;
        ExceptionType = exceptionType;
        ExceptionMessage = exceptionMessage;
        Token = token;
        RefreshToken = refreshToken;
    }

    public ValueConclusion()
    {
    }

    public override int? UniqueStatusCode { get; set; }
    public override string? Description { get; set; }
    public override string? UniqueInfo { get; set; }
    public override string? ExceptionId { get; set; }
    public override string? ExceptionType { get; set; }
    public override string? ExceptionMessage { get; set; }
    public override string? Token { get; set; }
    public override string? RefreshToken { get; set; }
}