using Liup.Authorization.Infrastructure.Middlewares;
using Liup.Authorization.Infrastructure.HeaderProtocol;
using Liup.Authorization.Api.ServiceInitializer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InitializeServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiter();
//app.UseAuthorization();
//app.UseAuthentication();
//app.UseMiddleware<ExceptionHandler>();
app.UseSecurityHeaders(Protocol.HeaderPolicyCollection());
app.UseHsts();
app.MapControllers();

await app.RunAsync();
