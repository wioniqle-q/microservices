using Auth.Application.AuthManager.Handlers;
using Auth.Application.AuthManager.Requests;
using Auth.Application.InsertManager.Handlers;
using Auth.Application.InsertManager.Requests;
using Auth.Application.ResetPwManager.Handlers;
using Auth.Application.ResetPwManager.Requests;
using Auth.Application.TransferManager.Handlers;
using Auth.Application.TransferManager.Requests;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Coa.Auth.Api.DependencyInjections;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class AuthManagerServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequestHandler<AuthUserRequest, OutcomeValue>, AuthUserHandler>();
        services.AddScoped<IRequestPreProcessor<AuthUserRequest>, AuthUserPreProcessor>();
        services.AddScoped<IValidator<AuthUserRequest>, AuthUserByIdValidator>();

        services.AddScoped<IRequestHandler<InsertUserRequest, OutcomeValue>, InsertUserHandler>();
        services.AddScoped<IRequestPreProcessor<InsertUserRequest>, InsertUserPreProcessor>();
        services.AddScoped<IValidator<InsertUserRequest>, InsertUserByIdValidator>();

        services.AddScoped<IRequestHandler<ResetPwUserRequest, OutcomeValue>, ResetPwUserHandler>();
        services.AddScoped<IRequestPreProcessor<ResetPwUserRequest>, ResetPwUserPreProcessor>();
        services.AddScoped<IValidator<ResetPwUserRequest>, ResetPwUserByIdValidator>();

        services.AddScoped<IRequestHandler<TransferUserRequest, OutcomeValue>, TransferUserHandler>();
        services.AddScoped<IRequestPreProcessor<TransferUserRequest>, TransferUserPreProcessor>();
        services.AddScoped<IValidator<TransferUserRequest>, TransferUserByIdValidator>();

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            x.Lifetime = ServiceLifetime.Transient;
        });
    }
}