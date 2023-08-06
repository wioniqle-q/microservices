using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

public interface IUserHelper
{
    Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<BaseUserEntitiy?> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateUserLastLoginAsync(BaseUserEntitiy user, DateTime currentLoginTime,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserPasswordAsync(string newPassword, string oldPassword, BaseUserEntitiy user,
        CancellationToken cancellationToken = default);

    Task<bool> SaveUserLastLoginAsync(BaseUserEntitiy user, CancellationToken cancellationToken = default);

    Task<OutComeValue> CreateUserAsync(BaseUserEntitiy user, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    Task<OutComeValue> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserVerifyEmailAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default);

    Task<string> UpdateUserTokenAsync(BaseUserEntitiy user, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default);
}