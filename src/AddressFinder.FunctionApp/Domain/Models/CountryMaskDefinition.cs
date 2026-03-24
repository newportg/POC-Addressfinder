namespace AddressFinder.FunctionApp.Domain.Models;

public sealed record CountryMaskDefinition
{
    public required string Country { get; init; }
    public required string Iso3char { get; init; }
    public required string Iso2char { get; init; }
    public required Dictionary<string, List<string>> Lines { get; init; }
    public required int MaxLine { get; init; }
    public bool SupportsPoBox { get; init; }
    public required string MaskVersion { get; init; }
    public required string MaskSource { get; init; }

    public static int DeriveMaxLine(IReadOnlyDictionary<string, List<string>> lines)
    {
        var max = 0;
        foreach (var key in lines.Keys)
        {
            if (key.StartsWith("Line", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(key[4..], out var number)
                && number > max)
            {
                max = number;
            }
        }

        return max;
    }
}
