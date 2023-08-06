using Auth.Infrastructure.TransferProtocol.ConclusionsInterfaces;

namespace Auth.Infrastructure.TransferProtocol.ConclusionsAbstraction;

public abstract class TransferOutcomeAbstract : ITransferOutcome
{
    public virtual bool? Status { get; set; }
    public virtual string? Description { get; set; }
}