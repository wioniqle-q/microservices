using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferAbstractions;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.TransferProtocol.TransferBlocks;

public sealed class QuerySection : QuerySectionAbstract
{
    private readonly IUserHelper _userHelper;

    public QuerySection(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }

    public override async Task<bool> CheckReuseToken(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default)
    {
        var quickSort = await HorsPool.HorspoolSearch(baseUserEntitiy.UserProperty.RefreshTokens.ToArray(), token);
        return quickSort;
    }

    public override async Task<bool> RemoveUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, baseUserEntitiy.UserName);

        var update = Builders<BaseUserEntitiy>.Update
            .Set(x => x.UserProperty.RefreshToken, string.Empty)
            .AddToSet(x => x.UserProperty.RefreshTokens, token);

        var updateUserAsync = await _userHelper.UpdateUserAsync(filter, update, null!, CancellationToken.None);
        if (updateUserAsync is false)
            return false;

        var checkRefreshTokens =
            await CheckRefreshTokensCount(baseUserEntitiy, CancellationToken.None);
        return checkRefreshTokens;
    }

    public override async Task<bool> AddUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, baseUserEntitiy.UserName);
        var update = Builders<BaseUserEntitiy>.Update
            .Set(x => x.UserProperty.RefreshToken, token);

        var updateUserAsync = await _userHelper.UpdateUserAsync(filter, update, null!, CancellationToken.None);
        return updateUserAsync;
    }

    public override async Task<bool> CheckRefreshTokensCount(BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default)
    {
        if (baseUserEntitiy.UserProperty.RefreshTokens.Count <= 10)
            return true;

        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, baseUserEntitiy.UserName);
        var update = Builders<BaseUserEntitiy>.Update
            .PopFirst(x => x.UserProperty.RefreshTokens);

        var popUpdate = await _userHelper.UpdateUserAsync(filter, update, null!, CancellationToken.None);
        return popUpdate;
    }
}