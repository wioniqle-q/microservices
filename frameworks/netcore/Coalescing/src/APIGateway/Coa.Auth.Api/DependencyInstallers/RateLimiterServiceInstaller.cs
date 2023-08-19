using System.Threading.RateLimiting;
using Coa.Auth.Api.DependencyInjections;
using Microsoft.Extensions.Primitives;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class RateLimiterServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Concurrency limit
        // Fixed window limit - Were Using this
        // Sliding windows limit
        // Token bucket limit - Were Using this too
        // Leaky bucket limit

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ipAddress = GetIpAddress(httpContext);
                if (ipAddress == null) return RateLimitPartition.GetNoLimiter("none");

                if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                    return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 20,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(10),
                        TokensPerPeriod = 100,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 30
                    });

                if (httpContext.Request.Headers.ContainsKey("CF-Connecting-IP"))
                    return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 20,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(10),
                        TokensPerPeriod = 100,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 30
                    });

                return RateLimitPartition.GetSlidingWindowLimiter(ipAddress, _ => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 40,
                    QueueLimit = 100,
                    SegmentsPerWindow = 50,
                    Window = TimeSpan.FromSeconds(60),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });
    }

    private static string? GetIpAddress(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"];
        return forwardedFor != StringValues.Empty
            ? forwardedFor.ToString().Split(',')[0]
            : httpContext.Connection.RemoteIpAddress?.ToString();
    }
}