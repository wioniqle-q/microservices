namespace Auth.Infrastructure.UserTransaction.Interfaces;

public interface IEndpointOption
{
    public string TransactionId => null!;
    public string EndpointId => null!;
    public string ClientName => null!;
    public string PermissionType => null!;
    public string PermissionName => null!;
    public string PermissionValue => null!;
    public string SpecialValue => null!;
    public string CreatedDate => null!;
    public string ExpirationDate => null!;
}