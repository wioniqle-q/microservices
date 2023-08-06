using Auth.Infrastructure.DigitalSignature.DigitalSignatureAbstracts;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureOptions;
using Auth.Infrastructure.DigitalSignature.DigitalSignatures;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class DigitalSignatureServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var userSignatureOptions = new UserSignatureOptions
        {
            SecretKey = configuration["UserSignatureOptions:SecretKey"] ?? string.Empty,
            Issuer = configuration["UserSignatureOptions:Issuer"] ?? string.Empty,
            Audience = configuration["UserSignatureOptions:Audience"] ?? string.Empty
        };
        var accessSignatureOptions = new AccessSignatureOptions
        {
            SecretKey = configuration["AccessSignatureOptions:SecretKey"] ?? string.Empty,
            Issuer = configuration["AccessSignatureOptions:Issuer"] ?? string.Empty,
            Audience = configuration["AccessSignatureOptions:Audience"] ?? string.Empty
        };

        services.AddSingleton(userSignatureOptions);
        services.AddSingleton(accessSignatureOptions);

        services.AddScoped<UserSignature>();
        services.AddScoped<IUserSignature, UserSignature>();
        services.AddScoped<UserSignatureAbstract, UserSignature>();

        services.AddScoped<AccessSignature>();
        services.AddScoped<IAccessSignature, AccessSignature>();
        services.AddScoped<AccessSignatureAbstract, AccessSignature>();
    }
}