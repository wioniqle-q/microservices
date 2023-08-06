using Auth.Infrastructure.TransferProtocol.ConclusionsAbstraction;

namespace Auth.Infrastructure.TransferProtocol.Conclusions;

public sealed class TransferOutcomeValue : TransferOutcomeAbstract
{
    public override bool? Status { get; set; }
    public override string? Description { get; set; }
}