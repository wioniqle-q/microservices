using System.Threading.RateLimiting;
using FluentValidation;
using Liup.Authorization.Application.Authorization.Manager.Handlers;
using Liup.Authorization.Application.Authorization.Manager.Requests;
using Liup.Authorization.Infrastructure.CredentialVerification.Interfaces;
using Liup.Authorization.Infrastructure.CredentialVerification.MongoDB.CVerification;
using Liup.Authorization.Infrastructure.Data.MongoDB.Initializer;
using Liup.Authorization.Infrastructure.Data.MongoDB.Options;
using Liup.Authorization.Infrastructure.GlobalEncryption.Encryption;
using Liup.Authorization.Infrastructure.GlobalEncryption.Interfaces;
using Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Liup.UserInteraction.Infrastructure.Data.Contexts;
using Liup.UserInteraction.Infrastructure.Data.Initializer;
using Liup.UserInteraction.Infrastructure.Data.Interfaces;
using Liup.UserInteraction.Infrastructure.Data.Modules;
using Liup.UserInteraction.Infrastructure.Data.Options;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory.InvestigationDisclosure;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Liup.Authorization.Api.ServiceInitializer;

public static class Initialize
{
    public static void InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.InitializeUserInteractionDatabase(configuration);
        services.InitializeAuthenticateDatabase(configuration);
        services.InitializeUserInvestigation();
        services.InitializeRabbitMq(configuration);
        services.InitializeRateLimiter();
        services.InitializePipelines();
        services.InitializeMediatR();
        services.InitializeVersionEra();
        services.InitializeLogger();
        services.InitializeCors();

        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    private static void InitializeMediatR(this IServiceCollection services)
    {
        services.AddSingleton<IRequestHandler<AuthenticateUserRequest, AuthenticateUserResult>, AuthenticateUserHandler>();
        services.AddSingleton<IRequestPreProcessor<AuthenticateUserRequest>, AuthenticateUserPreProcessor>();
        services.AddSingleton<IValidator<AuthenticateUserRequest>, AuthenticateUserByIdValidator>();

        services.AddMediatR(typeof(Program).Assembly);
    }

    private static void InitializePipelines(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(AuthenticateUserPipelineBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    private static void InitializeRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Request.Headers["CF-Connecting-IP"].ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 15,
                    Window = TimeSpan.FromSeconds(30)
                })
            );
        });
    }

    private static void InitializeRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMqConfig(
            configuration.GetSection("RabbitMqConfig:HostName").Value,
            configuration.GetSection("RabbitMqConfig:UserName").Value,
            configuration.GetSection("RabbitMqConfig:Password").Value,
            configuration.GetSection("RabbitMqConfig:QueueName1").Value);

        RabbitMqConfigure.AddRabbitMq(services, rabbitMqOptions);
    }

    private static void InitializeAuthenticateDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbOptions = new DataOptions(
            configuration.GetSection("MongoDbOptions:ConnectionString").Value,
            configuration.GetSection("MongoDbOptions:DatabaseName").Value);

        services.AddSingleton(mongoDbOptions);
        DataRegistrar.RegisterData(services);
    }

    private static void InitializeUserInteractionDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbOptions = new InteractionMongoDbSettings()
        {
            ConnectionString = configuration.GetSection("MongoDbOptions:ConnectionString").Value,
            DatabaseName = configuration.GetSection("MongoDbOptions:DatabaseName").Value,
        };
        services.AddSingleton(mongoDbOptions);

        services.AddSingleton<IInteractionMongoDbContextHelper, InteractionMongoDbContextHelper>();
        services.AddSingleton<IInteractionMongoDbContext, InteractionMongoDbContext>();
        services.AddSingleton<InteractionMongoDbModule>();
        services.AddTransient<InteractionMongoDbContextHelper>();
        services.AddSingleton<InteractionMongoDbContext>();

        services.AddSingleton<InteractionMongoDbContextHelper>();
    }

    public static void InitializeUserInvestigation(this IServiceCollection services)
    {
        services.AddSingleton<IUserInvestigation, UserInvestigation>();
        services.AddSingleton<InvestigationResult>();

        services.AddSingleton<IEncryptionProvider, EncryptionProvider>();
        services.AddSingleton<ICredentialAuthenticityAssessment, CredentialAuthenticityAssessment>();
    }

    private static void InitializeVersionEra(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version")
            );
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void InitializeLogger(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddLogging();
    }

    private static void InitializeCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });
    }
}
