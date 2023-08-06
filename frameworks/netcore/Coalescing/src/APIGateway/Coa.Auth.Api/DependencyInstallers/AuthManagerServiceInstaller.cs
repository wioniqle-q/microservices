using Auth.Application.AuthManager.Handlers;
using Auth.Application.AuthManager.Requests;
using Auth.Application.InsertManager.Handlers;
using Auth.Application.InsertManager.Requests;
using Auth.Application.ResetPwManager.Handlers;
using Auth.Application.ResetPwManager.Requests;
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
        services.AddTransient<IRequestHandler<AuthUserRequest, OutcomeValue>, AuthUserHandler>();
        services.AddTransient<IRequestPreProcessor<AuthUserRequest>, AuthUserPreProcessor>();
        services.AddTransient<IValidator<AuthUserRequest>, AuthUserByIdValidator>();

        services.AddTransient<IRequestHandler<InsertUserRequest, OutcomeValue>, InsertUserHandler>();
        services.AddTransient<IRequestPreProcessor<InsertUserRequest>, InsertUserPreProcessor>();
        services.AddTransient<IValidator<InsertUserRequest>, InsertUserByIdValidator>();

        services.AddTransient<IRequestHandler<ResetPwUserRequest, OutcomeValue>, ResetPwUserHandler>();
        services.AddTransient<IRequestPreProcessor<ResetPwUserRequest>, ResetPwUserPreProcessor>();
        services.AddTransient<IValidator<ResetPwUserRequest>, ResetPwUserByIdValidator>();

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            x.AddOpenBehavior(typeof(ResetPwUserPipelineBehavior<,>));
            x.AddOpenBehavior(typeof(InsertUserPipelineBehavior<,>));
            x.AddOpenBehavior(typeof(AuthUserPipelineBehavior<,>));
        });
    }
}