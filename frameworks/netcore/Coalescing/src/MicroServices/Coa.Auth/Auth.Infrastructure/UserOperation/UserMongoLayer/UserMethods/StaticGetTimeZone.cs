using System.Net;
using System.Net.Http.Headers;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;

public class StaticGetTimeZone : StaticGetTimeZoneAbstract
{
    private static readonly HttpClient Client = new();

    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = Policy
        .Handle<Exception>()
        .OrResult<HttpResponseMessage>(response => response.StatusCode != HttpStatusCode.OK)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public override async Task<string> GetUserTimeZoneAsIp(string ip)
    {
        var apiUrl = $"https://www.timeapi.io/api/Time/current/ip?ipAddress={ip}";

        try
        {
            if (!IsValidIpAddress(ip))
                return "Invalid IP address";

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            if (!IsSecureUrl(new Uri(apiUrl)))
                return "Insecure URL";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await RetryPolicy.ExecuteAsync(async () => await Client.SendAsync(request, cts.Token));
            if (!response.IsSuccessStatusCode)
                return "timeZone not found";

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (!string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase))
                return "Invalid content type";

            var content = await response.Content.ReadAsStringAsync(cts.Token);
            var jObject = JObject.Parse(content);
            var timeZone = jObject["timeZone"]!.ToString();

            return timeZone;
        }
        catch (TaskCanceledException)
        {
            return "Request timeout";
        }
        catch (Exception)
        {
            return "An error occurred";
        }
    }

    protected virtual bool IsValidIpAddress(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }

    protected virtual bool IsSecureUrl(Uri url)
    {
        return url.Scheme == Uri.UriSchemeHttps;
    }
}