using System.Linq.Expressions;
using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.Data.Interfaces;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory.InvestigationDisclosure;
using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;

public class UserInvestigation : IUserInvestigation
{
    private readonly IInteractionMongoDbContextHelper @ContextHelper;

    public UserInvestigation(IInteractionMongoDbContextHelper contextHelper)
    {
        @ContextHelper = contextHelper;
    }

    public virtual async Task<UserModel> FindOneAsync(UserModel user, int limit, CancellationToken cancellationToken = default)
    {
        return await @ContextHelper.FindOneAsync<UserModel>(d => d.UserName == user.UserName, limit, cancellationToken);
    }

    public virtual async Task<InvestigationResult> InvestigateUserInsertAsync(UserModel user, int limit, CancellationToken cancellationToken = default)
    {
        var userExists = await this.FindOneAsync(user, limit, cancellationToken);
        if (userExists is not null)
        {
            return new InvestigationResult("User already exists");
        }

        var hashString = await PasswordSecurity.PasswordSecurity.HashPassword(user.Password);

        var userModel = new UserModel
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            Password = hashString,
            UserProperties = new UserProperty
            {
                UserId = user.UserId,
                IsEmailConfirmed = false,
                IsLocked = false,
                IsDeleted = false,
                Token = null,
                CreatedDate = string.Concat(DateTime.Now),
                ModifiedDate = string.Concat(DateTime.Now)
            }
        };

        var sesion = await @ContextHelper.StartSessionAsync(cancellationToken);
        var result = await sesion.WithTransactionAsync(async (handle, token) =>
        {
            await @ContextHelper.InsertOneAsync(user, cancellationToken: cancellationToken);
            return new InvestigationResult("User added");
        }, cancellationToken: cancellationToken);

        await sesion.CommitTransactionAsync(cancellationToken);
        return result;
    }

    public async virtual Task<InvestigationResult> InvestigateUserUpdateAsync(UserModel userModel, UpdateDefinition<UserModel> updateDefinition, int limit, CancellationToken cancellationToken = default)
    {
        var userExists = this.FindOneAsync(userModel, limit, cancellationToken);
        if (userExists is null)
        {
            return await Task.FromResult(new InvestigationResult("User does not exist"));
        }

        Expression<Func<UserModel, bool>> predicate = @params => @params.UserId == userModel.UserId;

        var sesion = await @ContextHelper.StartSessionAsync(cancellationToken);
        var result = await sesion.WithTransactionAsync(async (handle, token) =>
        {
            await @ContextHelper.UpdateOneAsync(predicate, updateDefinition, cancellationToken: cancellationToken);
            return new InvestigationResult("User updated");
        }, cancellationToken: cancellationToken);

        await sesion.CommitTransactionAsync(cancellationToken);

        return result;
    }
}