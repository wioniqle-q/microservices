using System.Text.RegularExpressions;
using Auth.Infrastructure.SanitizeProtocol.SanitizeAbstractions;
using Ganss.Xss;
using Pomelo.AntiXSS;

namespace Auth.Infrastructure.SanitizeProtocol;

public sealed class BaseSanitize : SanitizeAbstract
{
    private readonly AntiXSS _antiXss = new(new DefaultWhiteListProvider(), new DefaultTagAuthorizationProvider());
    private readonly HtmlSanitizer _sanitizer = new();

    public override async Task<string> SanitizeAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var sanitizedValue = _antiXss.StripHtml(input);

        sanitizedValue = await SanitizeHtml(sanitizedValue).ConfigureAwait(false);
        sanitizedValue = Regex.Replace(sanitizedValue,
            @"alert\(1\)|&gt;|\]|\[|\)|&lt;&quot;|&amp;|&apos;|&nbsp;|&cent;|&pound;|&yen;|&euro;|&copy;|&reg;|&trade;|&times;|&divide;|&amp;#x[0-9a-fA-F]+;|&amp;#d+;|&amp;#0[0-9]+;|&amp;#[0-9]+;|&amp;#[xX][0-9a-fA-F]+;",
            string.Empty);

        return sanitizedValue;
    }

    private async Task<string> SanitizeHtml(string html)
    {
        return await Task.FromResult(_sanitizer.Sanitize(html));
    }
}