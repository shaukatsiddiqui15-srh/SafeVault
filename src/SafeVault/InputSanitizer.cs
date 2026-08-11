using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SafeVault;

public static class InputSanitizer
{
    private static readonly Regex DisallowedUsernameCharacters = new(@"[^a-zA-Z0-9._-]+", RegexOptions.Compiled);
    private static readonly Regex MultipleWhitespace = new(@"\s+", RegexOptions.Compiled);

    public static string SanitizeUsername(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Username is required.", nameof(input));
        }

        var normalized = input.Trim();
        normalized = normalized.Replace("<", string.Empty, StringComparison.Ordinal)
                               .Replace(">", string.Empty, StringComparison.Ordinal)
                               .Replace("\"", string.Empty, StringComparison.Ordinal)
                               .Replace("'", string.Empty, StringComparison.Ordinal)
                               .Replace(";", string.Empty, StringComparison.Ordinal)
                               .Replace("--", string.Empty, StringComparison.Ordinal);
        normalized = DisallowedUsernameCharacters.Replace(normalized, string.Empty);
        normalized = MultipleWhitespace.Replace(normalized, " ");

        if (normalized.Length is < 3 or > 50)
        {
            throw new ArgumentException("Username must be between 3 and 50 characters after sanitization.", nameof(input));
        }

        return normalized;
    }

    public static string SanitizeEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Email is required.", nameof(input));
        }

        var normalized = input.Trim();

        if (normalized.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Email cannot contain whitespace.", nameof(input));
        }

        if (normalized.Contains('<') || normalized.Contains('>') || normalized.Contains('"') || normalized.Contains('\''))
        {
            throw new ArgumentException("Email contains invalid characters.", nameof(input));
        }

        _ = new MailAddress(normalized);
        return normalized;
    }

    public static string HtmlEncodeForDisplay(string input)
    {
        return WebUtility.HtmlEncode(input ?? string.Empty);
    }
}
