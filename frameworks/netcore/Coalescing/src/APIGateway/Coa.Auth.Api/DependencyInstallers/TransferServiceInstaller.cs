using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferAbstractions;
using Auth.Infrastructure.TransferProtocol.TransferBlocks;
using Auth.Infrastructure.TransferProtocol.TransferConfigurations;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;
using Auth.Infrastructure.TransferProtocol.Transfers;
using Auth.Infrastructure.TransferProtocol.TransferUtilitiy;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class TransferServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var options = new CatConfiguration
        {
            SdkKey = configuration.GetSection("CatConfiguration:SdkKey").Value ?? string.Empty,
            Flags = configuration.GetSection("CatConfiguration:Flags").Get<string[]>() ?? Array.Empty<string>()
        };
        services.AddSingleton(options);

        services.AddScoped<Transfer>();
        services.AddScoped<ITransfer, Transfer>();
        services.AddScoped<TransferAbstract, Transfer>();

        services.AddScoped<DeviceComparer<BaseDevice>>();
        services.AddScoped<IDeviceComparer, DeviceComparer<BaseDevice>>();
        services.AddScoped<DeviceComparerAbstract, DeviceComparer<BaseDevice>>();

        services.AddScoped<ArtifactSection>();
        services.AddScoped<IArtifactSection, ArtifactSection>();
        services.AddScoped<ArtifactSectionAbstract, ArtifactSection>();

        services.AddScoped<QuerySection>();
        services.AddScoped<IQuerySection, QuerySection>();
        services.AddScoped<QuerySectionAbstract, QuerySection>();
    }
}