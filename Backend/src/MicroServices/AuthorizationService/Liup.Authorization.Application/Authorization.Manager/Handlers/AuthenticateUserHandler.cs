using Liup.Authorization.Application.Authorization.Manager.Requests;
using Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Liup.Authorization.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;
using MediatR;

namespace Liup.Authorization.Application.Authorization.Manager.Handlers;

public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResult>
{
    private readonly IEventBus _eventBus;

    private readonly IUserInvestigation _userInvestigation;

    public AuthenticateUserHandler(IEventBus eventBus, IUserInvestigation userInvestigation)
    {
        _eventBus = eventBus;
        _userInvestigation = userInvestigation;
    }

    public async Task<AuthenticateUserResult> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
    {

        var userId = Guid.NewGuid();

        var user = new UserModel
        {
            UserId = userId,
            UserName = request.UserName,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            UserProperties = new UserProperty
            {
                UserId = userId,
                IsEmailConfirmed = false,
                IsLocked = false,
                IsDeleted = false,
                Token = null,
                CreatedDate = DateTime.Now.ToString(),
                ModifiedDate = DateTime.Now.ToString()
            }
        };

        var investigationResult = await _userInvestigation.InvestigateUserInsertAsync(user, 1, cancellationToken);

        _eventBus.Publish<AuthenticatedUserEvent>(new AuthenticatedUserEvent
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email
        });

        return await Task.FromResult(new AuthenticateUserResult(investigationResult.InvestigationResponse));
    }
}