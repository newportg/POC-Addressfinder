using System.Text.RegularExpressions;

namespace AddressFinder.FunctionApp.Domain.Services;

public sealed class InputValidationService
{
    private static readonly Regex CountryCodeRegex = new("^[A-Z]{2}$", RegexOptions.Compiled);
    private static readonly Dictionary<string, string> Iso3ToIso2 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["USA"] = "US",
        ["GBR"] = "GB",
        ["CAN"] = "CA"
    };

    public string? ValidateCountryCode(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return "INVALID_COUNTRY";
        }

        return CountryCodeRegex.IsMatch(countryCode.Trim().ToUpperInvariant()) ? null : "INVALID_COUNTRY";
    }

    public string? ValidateAddressInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "INVALID_INPUT";
        }

        return input.Length > 1000 ? "INPUT_TOO_LONG" : null;
    }

    public string? InferCountryCode(string input)
    {
        var trimmed = input.Trim().ToUpperInvariant();
        if (trimmed.EndsWith(" USA") || trimmed.EndsWith(" US")) return "US";
        if (trimmed.EndsWith(" UNITED KINGDOM") || trimmed.EndsWith(" GB")) return "GB";
        return null;
    }

    public string? NormalizeCountryCode(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return null;
        }

        var normalized = countryCode.Trim().ToUpperInvariant();
        if (CountryCodeRegex.IsMatch(normalized))
        {
            return normalized;
        }

        return Iso3ToIso2.TryGetValue(normalized, out var iso2) ? iso2 : null;
    }
}
