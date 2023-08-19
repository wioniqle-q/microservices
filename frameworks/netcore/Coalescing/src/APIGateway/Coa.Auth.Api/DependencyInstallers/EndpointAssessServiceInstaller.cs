using Auth.Infrastructure.UserTransaction.EndpointOptions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using Coa.Auth.Api.DependencyInjections;
using Coa.Shared.PermBehavior.EndpointAccessors;
using Coa.Shared.PermBehavior.EndpointAccessors.AccessorsAbstractions;
using Coa.Shared.PermBehavior.EndpointAccessors.AccessorsInterfaces;
using Coa.Shared.PermBehavior.EndpointAdapters.Adapters;
using Coa.Shared.PermBehavior.EndpointAdapters.AdaptersAbstractions;
using Coa.Shared.PermBehavior.EndpointAdapters.AdaptersInterfaces;
using Coa.Shared.PermBehavior.EndpointCredentials;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsAbstractions;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsAbstractions;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;
using Coa.Shared.PermBehavior.EndpointValidators;
using Coa.Shared.PermBehavior.EndpointValidators.ValidatorsAbstractions;
using Coa.Shared.PermBehavior.EndpointValidators.ValidatorsInterfaces;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class EndpointAssessServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var credentialOptions = new CredentialOptions();
        configuration.GetSection("CredentialOptions").Bind(credentialOptions);
        services.AddSingleton(credentialOptions);

        var endpointOptions = new EndpointOption();
        configuration.GetSection("EndpointOptions").Bind(endpointOptions);

        services.AddScoped<IEndpointOption, EndpointOption>();
        services.AddSingleton(endpointOptions);

        services.AddScoped<ValueConclusion>();
        services.AddScoped<IConclusions, ValueConclusion>();
        services.AddScoped<ConclusionsAbstract, ValueConclusion>();

        services.AddScoped<EndpointCredential>();
        services.AddScoped<IEndpointCredential, EndpointCredential>();
        services.AddScoped<EndpointCredentialAbstract, EndpointCredential>();

        services.AddScoped<EndpointAccessor>();
        services.AddScoped<IEndpointAccessor, EndpointAccessor>();
        services.AddScoped<EndpointAccessorAbstract, EndpointAccessor>();

        services.AddScoped<EndpointValidate>();
        services.AddScoped<IEndpointValidate, EndpointValidate>();
        services.AddScoped<EndpointValidateAbstract, EndpointValidate>();

        services.AddScoped<InitPermAdapter>();
        services.AddScoped<IEndpointAdapter, InitPermAdapter>();
        services.AddScoped<EndpointAdapterAbstract, InitPermAdapter>();
    }
}