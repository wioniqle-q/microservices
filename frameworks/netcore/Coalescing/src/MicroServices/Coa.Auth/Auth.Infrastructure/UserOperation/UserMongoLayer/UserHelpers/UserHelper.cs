using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.PasswordObfuscation;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserHelpers;

public sealed class UserHelper : UserHelperAbstract
{
    private readonly IConcealment _concealment;
    private readonly IUserOperation _userOperation;
    private readonly IUserSignature _userSignature;

    public UserHelper(IUserOperation userOperation, IUserSignature userSignature, IConcealment concealment)
    {
        _userOperation = userOperation;
        _userSignature = userSignature;
        _concealment = concealment;
    }

    public override async Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userOperation.GetUserByIdAsync(userId, cancellationToken);
        return user;
    }

    public override async Task<BaseUserEntitiy?> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userOperation.FindUserByQueryAsync(query, cancellationToken);
        return user;
    }

    public override async Task<List<BaseUserEntitiy>> FindUsersByQueryWithPageAsync(int skip, int limit,
        CancellationToken cancellationToken = default)
    {
        var users = await _userOperation.FindUsersByQueryWithPageAsync(skip, limit, cancellationToken);
        return users ?? new List<BaseUserEntitiy>();
    }

    public override async Task<OutComeValue> CreateUserAsync(BaseUserEntitiy user,
        BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);
        var userExist = await FindUserByQueryAsync(filter, cancellationToken);
        if (userExist is not null)
            return new OutComeValue
            {
                UniqueResId = Guid.NewGuid().ToString(),
                Description = "User already exist"
            };

        var obfuscatePassword = await ObfuscatePassword.Obfuscate(user.Password).ConfigureAwait(true);
        var concealedPassword = await _concealment.ConcealAsync(obfuscatePassword, null, null);

        var genSignature =
            await _userSignature.GenerateUserToken(string.Concat(user.UserId), signatureEntitiy.TrialDate,
                signatureEntitiy);

        var userRsa = new BaseUserRsa
        {
            RsaPublicKey = user.UserRsa.RsaPublicKey,
            RsaPrivateKey = user.UserRsa.RsaPrivateKey,
            RsaValidateKey = user.UserRsa.RsaValidateKey
        };

        var userDevice = new BaseDevice
        {
            UserId = user.Device.UserId,
            DeviceId = user.Device.DeviceId,
            DeviceType = user.Device.DeviceType,
            DeviceLocation = user.Device.DeviceLocation,
            DeviceLocationTimeZone = user.Device.DeviceLocationTimeZone,
            DeviceNetworkName = user.Device.DeviceNetworkName,
            DeviceNetworkIp = user.Device.DeviceNetworkIp,
            DeviceNetworkMac = user.Device.DeviceNetworkMac,
            DeviceHardwareId = user.Device.DeviceHardwareId,
            DeviceHardwareMotherboardName = user.Device.DeviceHardwareMotherboardName,
            DeviceHardwareOsName = user.Device.DeviceHardwareOsName,
            DeviceProcessorName = user.Device.DeviceProcessorName,
            DeviceGraphicsName = user.Device.DeviceGraphicsName
        };

        var userProperties = new BaseUserProperty
        {
            UserId = user.UserId,
            IsEmailConfirmed = user.UserProperty.IsEmailConfirmed,
            IsLocked = user.UserProperty.IsLocked,
            IsDeleted = user.UserProperty.IsDeleted,
            Token = genSignature,
            CreatedAt = user.UserProperty.CreatedAt,
            LastLogin = user.UserProperty.LastLogin,
            TimeZone = user.UserProperty.TimeZone,
            LoginTimeSpan = user.UserProperty.LoginTimeSpan,
            Require2Fa = user.UserProperty.Require2Fa,
            DeviceId = user.UserProperty.DeviceId,
            AccessToken = user.UserProperty.AccessToken,
            RefreshToken = user.UserProperty.RefreshToken,
            RefreshTokens = user.UserProperty.RefreshTokens
        };

        var userEntitiy = new BaseUserEntitiy
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            Password = concealedPassword,
            UserProperty = userProperties,
            Device = userDevice,
            UserRsa = userRsa
        };

        var result = await _userOperation.CreateUserAsync(userEntitiy, cancellationToken);
        if (result is false)
            return new OutComeValue
            {
                UniqueResId = Guid.NewGuid().ToString(),
                Description = "User creation failed"
            };

        return new OutComeValue
        {
            UniqueResId = Guid.NewGuid().ToString(),
            Description = "User created successfully",
            AccessToken = "",
            RefreshToken = userProperties.RefreshToken
        };
    }

    public override async Task<OutComeValue> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default)
    {
        // THIS CODE WILL BE REFACTOR LATER

        var result = await _userOperation.CreateManyUserAsync(users, cancellationToken);
        if (result is false)
            return new OutComeValue
            {
                UniqueResId = Guid.NewGuid().ToString(),
                Description = "User creation failed"
            };

        return new OutComeValue
        {
            UniqueResId = Guid.NewGuid().ToString(),
            Description = "User created successfully"
        };
    }


    public override async Task<bool> ValidateUserLastLoginAsync(BaseUserEntitiy user, DateTime currentLoginTime,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);
        var userExist = await FindUserByQueryAsync(filter, cancellationToken);
        if (userExist?.UserProperty is { IsLocked: true, IsDeleted: not true })
            return false;

        var lastLoginTime = userExist?.UserProperty?.LastLogin;
        var loginTimeSpan = userExist?.UserProperty?.LoginTimeSpan;

        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(user.UserProperty.TimeZone);
        var userLocalTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, userTimeZone);
        var userUtcTime = userLocalTime.ToUniversalTime();

        if (string.IsNullOrWhiteSpace(lastLoginTime) || loginTimeSpan <= 0)
        {
            var update = Builders<BaseUserEntitiy>
                .Update
                .Set(x => x.UserProperty.LastLogin, string.Concat(userUtcTime));

            var result = await _userOperation.UpdateUserAsync(filter, update, cancellationToken);
            if (result is false)
                return false;

            return false;
        }

        var lastLoginDateTime = DateTime.Parse(lastLoginTime);

        // Önceki oturum açma zamanından geriye doğru oturum açılmasını engellemek için...
        // To prevent logging in backwards from the previous logon time...
        if (currentLoginTime < lastLoginDateTime)
            // kullanıcının oturum açma zamanı aralığı hesaplanır
            // Kullanıcının lastLoginDateTime'ı, currentLoginTime'ın gerisinde olamaz.
            // Sebebi ise kullanıcının lastLoginDateTime'ı, currentLoginTime'ın gerisinde olursa
            // kullanıcı daha önceki bir tarihte oturum açmış olur.
            // Bu durumda kullanıcıya oturum açma zamanı aralığı hesaplanır ve bu aralıkta oturum açması engellenir.
            // Previous dememin sebebi ise kullanıcının lastLoginDateTime'ı, currentLoginTime'ın gerisinde olamaz.
            // Kullanıcının lastLoginDateTime'ı, currentLoginTime'ın gerisinde olursa kullanıcı daha önceki bir tarihte
            // oturum açmış olur. Bu durumda kullanıcıya oturum açma zamanı aralığı hesaplanır ve bu aralıkta oturum açması engellenir.
            // ================================================================================================================
            // the user's login time interval is calculated
            // The user's lastLoginDateTime cannot be behind the currentLoginTime.
            // The reason is that if the user's lastLoginDateTime is behind the currentLoginTime
            // the user is logged in at an earlier date.
            // In this case, a login time interval is calculated and the user is prevented from logging in during this interval.
            // The reason I say previous is that the user's lastLoginDateTime cannot be behind the currentLoginTime.
            // If the user's lastLoginDateTime is behind the currentLoginTime, the user will be logged in on an earlier date.
            // is logged in. In this case, a login time interval is // calculated and the user is prevented from logging in during this interval.
            return false;

        var threshold = CalculateDynamicThreshold(userExist!);
        var logoutDateTime = lastLoginDateTime + threshold;

        if (currentLoginTime > logoutDateTime)
            // kullanıcının oturum açma zamanı aralığı hesaplanır
            // kullanıcının lastLoginDateTime + threshold'ı, currentLoginTime'ın ilerisinde olamaz.
            // Sebebi ise kullanıcının lastLoginDateTime + threshold'ı, currentLoginTime'ın ilerisinde olursa
            // Kllanıcı son oturum açma zamanını geçmişte bırakarak ileri bir tarihte oturum açma girişimi yapmış olur
            // Before dememizin sebebi ise kullanıcının lastLoginDateTime + threshold'ı, currentLoginTime'ın ilerisinde olamaz.
            // Kullanıcının lastLoginDateTime + threshold'ı, currentLoginTime'ın ilerisinde olursa
            // Kllanıcı son oturum açma zamanını geçmişte bırakarak ileri bir tarihte oturum açma girişimi yapmış olur
            // ================================================================================================================
            // the user's login time interval is calculated
            // the user's lastLoginDateTime + threshold cannot be later than the currentLoginTime.
            // The reason is that if the user's lastLoginDateTime + threshold is later than the currentLoginTime
            // The user attempts to log in at a future date, leaving the last login time in the past
            // The reason we say Before is that the user's lastLoginDateTime + threshold cannot be later than the currentLoginTime.
            // If the user's lastLoginDateTime + threshold is later than the currentLoginTime
            // The user attempts to log in at a future date, leaving the last login time in the past
            return false;

        var updateLastLogin = Builders<BaseUserEntitiy>
            .Update
            .Set(x => x.UserProperty.LastLogin, string.Concat(userUtcTime));

        var resultLastLogin = await _userOperation.UpdateUserAsync(filter, updateLastLogin, cancellationToken)
            ;
        if (resultLastLogin is false)
            return false;

        return true;
    }

    public override async Task<bool> UpdateUserPasswordAsync(string newPassword, string oldPassword,
        BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var userFilter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);
        var userExist = await FindUserByQueryAsync(userFilter, cancellationToken);
        if (userExist?.UserProperty is
            { IsLocked: true, IsDeleted: true, Require2Fa: true, IsEmailConfirmed: not true })
            return false;

        var revealOldPassword = await _concealment.RevealAsync(userExist!.Password, null, null);

        var verifyPassword = await ObfuscatePassword.Verify(revealOldPassword, oldPassword);
        if (verifyPassword is false)
            return false;

        var verifyNewPassword = await ObfuscatePassword.Verify(revealOldPassword, newPassword);
        if (verifyNewPassword)
            return false;

        var obfuscateNewPassword = await ObfuscatePassword.Obfuscate(newPassword);
        var concealNewPassword =
            await _concealment.ConcealAsync(obfuscateNewPassword, null, null);
        var updatePassword = Builders<BaseUserEntitiy>
            .Update
            .Set(x => x.Password, concealNewPassword);

        var result = await _userOperation.UpdateUserAsync(userFilter, updatePassword, cancellationToken)
            ;
        return result;
    }

    public override async Task<bool> SaveUserLastLoginAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        if (user.UserProperty.Require2Fa is not true)
            return true;

        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);
        var userExist = await FindUserByQueryAsync(filter, cancellationToken);
        if (userExist?.UserProperty is { IsLocked: false, Require2Fa: not true })
            return true;

        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(user.UserProperty.TimeZone);
        var userLocalTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, userTimeZone);
        var userUtcTime = userLocalTime.ToUniversalTime();

        var update = Builders<BaseUserEntitiy>
            .Update
            .Set(x => x.UserProperty.LastLogin, string.Concat(userUtcTime))
            .Set(x => x.UserProperty.Require2Fa, false);

        var result = await _userOperation.UpdateUserAsync(filter, update, cancellationToken);
        return result;
    }

    public override async Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, BaseUserSignatureEntitiy signatureEntitiy,
        CancellationToken cancellationToken = default)
    {
        var result = await _userOperation.UpdateUserAsync(filter, update, cancellationToken);
        return result;
    }

    public override async Task<bool> UpdateUserVerifyEmailAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default)
    {
        var result = await _userOperation.UpdateUserAsync(filter, update, cancellationToken);
        return result;
    }

    public override async Task<string> UpdateUserTokenAsync(BaseUserEntitiy user,
        BaseUserSignatureEntitiy signatureEntitiy, CancellationToken cancellationToken = default)
    {
        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);

        var userExist = await FindUserByQueryAsync(filter, cancellationToken);
        if (userExist?.UserProperty is { IsLocked: true, IsDeleted: not true })
            return "Token verification failed, You are not authorized to perform this action, please contact admin";

        var genSignature = await _userSignature
            .GenerateUserToken(userExist?.UserId.ToString(), signatureEntitiy.TrialDate, signatureEntitiy);

        var update = Builders<BaseUserEntitiy>
            .Update
            .Set(x => x.UserProperty.Token, genSignature);

        var result = await _userOperation.UpdateUserAsync(filter, update, cancellationToken);
        return result
            ? "Your session has expired. Please login again"
            : "Session failed, please contact admin";
    }

    private static TimeSpan CalculateDynamicThreshold(IBaseUser user)
    {
        var lastLoginTime = user.UserProperty.LastLogin;
        if (string.IsNullOrWhiteSpace(lastLoginTime)) return TimeSpan.Zero;

        var currentLoginTime = DateTime.UtcNow;
        var timeSinceLastLogin = currentLoginTime - DateTime.Parse(lastLoginTime);

        var loginTimeSpan = user.UserProperty.LoginTimeSpan;
        if (loginTimeSpan <= 0) return TimeSpan.Zero;

        var dynamicThreshold = loginTimeSpan * timeSinceLastLogin.TotalMinutes / 60;
        var thresholdMinutes = TimeSpan.FromMinutes(dynamicThreshold);

        var lastLoginDateTime = DateTime.Parse(lastLoginTime);
        var lastLoginHour = lastLoginDateTime.Hour;
        var currentHour = currentLoginTime.Hour;

        if (lastLoginHour != currentHour) return thresholdMinutes;

        var currentMinute = currentLoginTime.Minute;
        switch (currentMinute)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                thresholdMinutes = TimeSpan.FromMinutes(5);
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                thresholdMinutes = TimeSpan.FromMinutes(15);
                break;
            default:
                thresholdMinutes = TimeSpan.FromMinutes(loginTimeSpan * timeSinceLastLogin.TotalSeconds / 60);
                break;
        }

        return thresholdMinutes;
    }
}