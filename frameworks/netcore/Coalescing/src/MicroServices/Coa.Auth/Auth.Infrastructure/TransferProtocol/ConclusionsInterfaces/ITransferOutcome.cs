namespace Auth.Infrastructure.TransferProtocol.ConclusionsInterfaces;

public interface ITransferOutcome
{
    public bool? Status { get; set; }
    public string? Description { get; set; }
}