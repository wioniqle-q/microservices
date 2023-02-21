using Liup.Authorization.Application.Authorization.Manager.Requests;
using Liup.Authorization.Domain.Models.MongoModels;
using Liup.Authorization.Infrastructure.CredentialVerification.Interfaces;
using Liup.Authorization.Infrastructure.GlobalEncryption.Encryption;
using Liup.Authorization.Infrastructure.GlobalEncryption.Interfaces;
using Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Liup.Authorization.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;
using MediatR;
using MongoDB.Driver;

namespace Liup.Authorization.Application.Authorization.Manager.Handlers;

public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResult>
{
    private readonly IEventBus _eventBus;

    private readonly IUserInvestigation _userInvestigation;

    private readonly ICredentialAuthenticityAssessment _credentialAuthenticityAssessment;

    public AuthenticateUserHandler(IEventBus eventBus, IUserInvestigation userInvestigation, ICredentialAuthenticityAssessment credentialAuthenticityAssessment)
    {
        _eventBus = eventBus;
        _userInvestigation = userInvestigation;
        _credentialAuthenticityAssessment = credentialAuthenticityAssessment;
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
            UserProperties = new UserInteraction.Domain.Models.MongoModels.UserProperty
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

        var update = Builders<UserModel>.Update.Set("UserProperties.IsEmailConfirmed", true);

        var investigationResult = await _userInvestigation.InvestigateUserInsertAsync(user, 1, cancellationToken);

        var investigationResultUpdate = await _userInvestigation.InvestigateUserUpdateAsync(user, update, 1, cancellationToken);

        Console.WriteLine(investigationResultUpdate.InvestigationResponse);

        var x = await _credentialAuthenticityAssessment.AssessCredentialAuthenticityAsync(new UserModelAssesment() { UserName = user.UserName, Password = user.Password }, cancellationToken);
        Console.WriteLine(x.CredentialResponse);

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
