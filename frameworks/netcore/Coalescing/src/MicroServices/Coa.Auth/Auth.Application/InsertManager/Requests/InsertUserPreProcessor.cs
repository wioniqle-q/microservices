using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.SanitizeProtocol.SanitizeInterfaces;
using MediatR.Pipeline;

namespace Auth.Application.InsertManager.Requests;

public sealed class InsertUserPreProcessor : IRequestPreProcessor<InsertUserRequest>
{
    private readonly IConcealment _concealment;
    private readonly ISanitize _sanitize;

    public InsertUserPreProcessor(IConcealment concealment, ISanitize sanitize)
    {
        _concealment = concealment;
        _sanitize = sanitize;
    }

    public async Task Process(InsertUserRequest request, CancellationToken cancellationToken)
    {
        var properties = request.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(string))
                continue;

            var value = (string)property.GetValue(request)!;

            var revealValue = await _concealment.RevealAsync(value, null, null).ConfigureAwait(false);
            var cleanUpValue = await _sanitize.SanitizeAsync(revealValue).ConfigureAwait(false);

            property.SetValue(request, cleanUpValue);
        }

        await Task.CompletedTask;
    }
}