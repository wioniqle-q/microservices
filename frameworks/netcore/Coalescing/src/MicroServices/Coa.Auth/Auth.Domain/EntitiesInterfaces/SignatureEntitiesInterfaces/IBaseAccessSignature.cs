namespace Auth.Domain.EntitiesInterfaces.SignatureEntitiesInterfaces;

public interface IBaseAccessSignature
{
    string TransactionId { get; set; }
    bool TrialStatus { get; set; }
    string UserId { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
    bool IsAccess { get; set; }
    string CustomAuthorization { get; set; }
    string OccurrenceTime { get; set; }
    string EnrollmentDate { get; set; }
}