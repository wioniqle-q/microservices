using Auth.Infrastructure.HeaderProtocol;
using Coa.Auth.Api.DependencyInjections;
using Coa.Auth.Api.DependencyInstallers;

var builder = WebApplication.CreateBuilder(args);

var assemblies = new[]
{
    typeof(ConcealmentProtocolServiceInstaller).Assembly,
    typeof(CredentialAssesmentServiceInstaller).Assembly,
    typeof(TransferAssesmentServiceInstaller).Assembly,
    typeof(DigitalSignatureServiceInstaller).Assembly,
    typeof(ResetPwAssesmentServiceInstaller).Assembly,
    typeof(InsertAssesmentServiceInstaller).Assembly,
    typeof(EndpointAssessServiceInstaller).Assembly,
    typeof(ApiVersioningServiceInstaller).Assembly,
    typeof(ETransactionServiceInstaller).Assembly,
    typeof(AuthManagerServiceInstaller).Assembly,
    typeof(RateLimiterServiceInstaller).Assembly,
    typeof(ValidationServiceInstaller).Assembly,
    typeof(SanitizeServiceInstaller).Assembly,
    typeof(TransferServiceInstaller).Assembly,
    typeof(AdlemanServiceInstaller).Assembly,
    typeof(MongoDbServiceInstaller).Assembly,
    typeof(TotpServiceInstaller).Assembly,
    typeof(GuidServiceInstaller).Assembly,
    typeof(ApiModuleInstaller).Assembly
};


builder.Services.InstallServices(builder.Configuration, assemblies);
builder.Services.InstallMessageBroker(builder.Configuration);

var app = builder.Build();

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseHeaderProtocol();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.UseHsts();

await app.RunAsync();