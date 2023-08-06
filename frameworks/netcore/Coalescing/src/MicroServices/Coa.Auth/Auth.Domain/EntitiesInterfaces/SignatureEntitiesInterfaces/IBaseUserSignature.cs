namespace Auth.Domain.EntitiesInterfaces.SignatureEntitiesInterfaces;

public interface IBaseUserSignature
{
    string TransactionId { get; set; }
    bool TrialStatus { get; set; }
    bool IsAuthorized { get; set; }
    string CustomAuthorization { get; set; }
    bool IsBlocked { get; set; }
    string OccurrenceTime { get; set; }
    string EnrollmentDate { get; set; }
    DateTime TrialDate { get; set; }
}