using Auth.Application.TransferManager.Requests;
using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using MediatR;

namespace Auth.Application.TransferManager.Handlers;

public sealed class TransferUserHandler : IRequestHandler<TransferUserRequest, OutcomeValue>
{
    private readonly IConcealment _baseConcealment;
    private readonly IInsertAssesment _insertAssesment;
    private readonly ITransferAssesment _transferAssesment;

    public TransferUserHandler(
        IInsertAssesment insertAssesment,
        IConcealment concealment,
        ITransferAssesment transferAssesment)
    {
        _insertAssesment = insertAssesment;
        _baseConcealment = concealment;
        _transferAssesment = transferAssesment;
    }


    public async Task<OutcomeValue> Handle(TransferUserRequest request, CancellationToken cancellationToken)
    {
        var transferRequest = new BaseUserEntitiy
        {
            UserProperty = new BaseUserProperty
            {
                AccessToken = request.AccessToken, 
                RefreshToken =request.RefreshToken
            }
        };

        var transferAssesment = await _transferAssesment.AssessTransferAsync(transferRequest);

        return await Task.FromResult(new OutcomeValue
        {
            UniqueResId = transferAssesment.UniqueResId,
            Outcome = transferAssesment.Outcome,
            Description = transferAssesment.Description,
            AccessToken = transferAssesment.AccessToken,
            RefreshToken = transferAssesment.RefreshToken
        });
    }
}