using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.EndpointOptions;

public sealed class EndpointOption : IEndpointOption
{
    public string TransactionId { get; set; } = null!;
    public string EndpointId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string PermissionType { get; set; } = null!; 
    public string PermissionName { get; set; } = null!;
    public string PermissionValue { get; set; } = null!;
    public string SpecialValue { get; set; } = null!;
    public string CreatedDate { get; set; } = null!;
    public string ExpirationDate { get; set; } = null!;
}