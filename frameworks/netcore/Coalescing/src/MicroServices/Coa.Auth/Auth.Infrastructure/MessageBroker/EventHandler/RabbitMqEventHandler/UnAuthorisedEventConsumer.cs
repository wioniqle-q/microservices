using System.Security.Cryptography;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.EmailTransaction.EmailTransactionInterfaces;
using Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;

namespace Auth.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;

public sealed class UnAuthorisedEventConsumer : IConsumer<UnAuthorisedEvent>
{
    private readonly IConcealment _baseConcealment;
    private readonly BaseUrlOptions _baseUrlOptions;
    private readonly IETransaction _eBaseTransaction;
    private readonly IUserSignature _userSignature;

    public UnAuthorisedEventConsumer(IETransaction eBaseTransaction, BaseUrlOptions baseUrlOptions,
        IUserSignature userSignature, IConcealment baseConcealment)
    {
        _eBaseTransaction = eBaseTransaction;
        _baseUrlOptions = baseUrlOptions;
        _userSignature = userSignature;
        _baseConcealment = baseConcealment;
    }

    public async Task Consume(ConsumeContext<UnAuthorisedEvent> context)
    {
        var emailVerifyId = await GenerateRandomHexString(8);
        var currentTime = DateTime.UtcNow;
        var updatedTime = currentTime.AddMinutes(20);

        var baseUserSignature = new BaseUserSignatureEntitiy
        {
            TransactionId = emailVerifyId,
            TrialStatus = true,
            IsAuthorized = false,
            CustomAuthorization = "EMAIL-USER",
            IsBlocked = false,
            OccurrenceTime = currentTime.ToString("O"),
            EnrollmentDate = updatedTime.ToString("O"),
            TrialDate = updatedTime
        };

        var generatedVerifyEmailToken = await _userSignature.GenerateUserToken(
            string.Concat(context.Message.UserProperty.UserId), baseUserSignature.TrialDate, baseUserSignature);
        if (string.IsNullOrEmpty(generatedVerifyEmailToken))
            await Task.FromCanceled(new CancellationToken(true));

        var userNameConceal = await _baseConcealment.ConcealAsync(context.Message.UserName, null, null);
        var tokenConceal = await _baseConcealment.ConcealAsync(generatedVerifyEmailToken, null, null);

        var emailVerifyUrl =
            $"{_baseUrlOptions.Host}/email/verify-email?userName={userNameConceal}&signature={tokenConceal}";

        var emailBody = $"Please click on the below link to verify your email address. {emailVerifyUrl}";

        await _eBaseTransaction.SendEmailAsync(context.Message.Email, "Email Verification", emailBody);

        await Task.CompletedTask;
    }

    private static async ValueTask<string> GenerateRandomHexString(int length = 36)
    {
        var randomBytes = new byte[length / 2];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return await new ValueTask<string>(BitConverter.ToString(randomBytes).Replace("-", string.Empty).ToLower());
    }
}