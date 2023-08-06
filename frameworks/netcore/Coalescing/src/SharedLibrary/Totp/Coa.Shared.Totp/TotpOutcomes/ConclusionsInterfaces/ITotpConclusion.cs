namespace Coa.Shared.Totp.TotpOutcomes.ConclusionsInterfaces;

public interface ITotpConclusion
{
    int? UniqueStatusCode { get; set; }
    int? TotpCode { get; set; }
    string? Description { get; set; }
}