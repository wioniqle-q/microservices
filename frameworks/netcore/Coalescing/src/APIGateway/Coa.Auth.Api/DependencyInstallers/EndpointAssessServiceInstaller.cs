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

        services.AddTransient<IEndpointOption, EndpointOption>();
        services.AddSingleton(endpointOptions);

        services.AddTransient<ValueConclusion>();
        services.AddTransient<IConclusions, ValueConclusion>();
        services.AddTransient<ConclusionsAbstract, ValueConclusion>();

        services.AddTransient<EndpointCredential>();
        services.AddTransient<IEndpointCredential, EndpointCredential>();
        services.AddTransient<EndpointCredentialAbstract, EndpointCredential>();

        services.AddTransient<EndpointAccessor>();
        services.AddTransient<IEndpointAccessor, EndpointAccessor>();
        services.AddTransient<EndpointAccessorAbstract, EndpointAccessor>();

        services.AddTransient<EndpointValidate>();
        services.AddTransient<IEndpointValidate, EndpointValidate>();
        services.AddTransient<EndpointValidateAbstract, EndpointValidate>();

        services.AddTransient<InitPermAdapter>();
        services.AddTransient<IEndpointAdapter, InitPermAdapter>();
        services.AddTransient<EndpointAdapterAbstract, InitPermAdapter>();
    }
}