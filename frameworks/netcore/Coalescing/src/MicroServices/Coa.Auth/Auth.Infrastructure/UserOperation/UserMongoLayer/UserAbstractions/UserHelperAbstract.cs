using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

public abstract class UserHelperAbstract : IUserHelper
{
    public abstract Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    public abstract Task<BaseUserEntitiy?> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default);
    
    public abstract Task<List<BaseUserEntitiy>> FindUsersByQueryWithPageAsync(int skip, int limit,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> ValidateUserLastLoginAsync(BaseUserEntitiy user, DateTime currentLoginTime,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> UpdateUserPasswordAsync(string newPassword, string oldPassword, BaseUserEntitiy user,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> SaveUserLastLoginAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default);

    public abstract Task<OutComeValue> CreateUserAsync(BaseUserEntitiy user, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    public abstract Task<OutComeValue> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> UpdateUserVerifyEmailAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update,
        CancellationToken cancellationToken = default);

    public abstract Task<string> UpdateUserTokenAsync(BaseUserEntitiy user, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);
}