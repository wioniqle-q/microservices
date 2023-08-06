namespace Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsInterfaces;

public interface IConclusions
{
    int? UniqueStatusCode { get; set; }
    string? Description { get; set; }
    string? UniqueInfo { get; set; }

    string? ExceptionId { get; set; }
    string? ExceptionType { get; set; }
    string? ExceptionMessage { get; set; }

    string? Token { get; set; }
    string? RefreshToken { get; set; }
}