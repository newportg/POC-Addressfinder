using System.Text.Json;
using System.Text.Json.Serialization;
using AddressFinder.FunctionApp.Domain.Models;

namespace AddressFinder.FunctionApp.Infrastructure.Catalog;

public sealed class EmbeddedMaskCatalogProvider : IEmbeddedMaskCatalogProvider
{
    private readonly Dictionary<string, CatalogCountry> _catalogByIso2;
    private readonly Dictionary<string, CatalogCountry> _catalogByIso3;
    private readonly Dictionary<string, List<string>> _fallbackLayout;
    private readonly string _catalogVersion;
    private readonly string _maskSource;

    public EmbeddedMaskCatalogProvider()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Catalog", "country-mask-catalog.json");
        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var root = JsonSerializer.Deserialize<CatalogRoot>(json, options)
                   ?? throw new InvalidOperationException("Mask catalog could not be parsed.");

        _catalogVersion = root.CatalogVersion;
        _maskSource = root.MaskSource;
        _fallbackLayout = root.FallbackLayout;

        _catalogByIso2 = new Dictionary<string, CatalogCountry>(StringComparer.OrdinalIgnoreCase);
        _catalogByIso3 = new Dictionary<string, CatalogCountry>(StringComparer.OrdinalIgnoreCase);

        foreach (var country in root.Countries)
        {
            _catalogByIso2[country.Iso2char] = country;
            _catalogByIso3[country.Iso3char] = country;
        }
    }

    public IReadOnlyDictionary<string, CountryMaskDefinition> GetAll()
    {
        return _catalogByIso2.ToDictionary(
            kv => kv.Key,
            kv => BuildMaskDefinition(kv.Value, MaskResolutionStatus.Exact),
            StringComparer.OrdinalIgnoreCase);
    }

    public MaskResolutionResult Resolve(string countryCode)
    {
        var normalized = countryCode.Trim().ToUpperInvariant();
        var country = ResolveCountry(normalized);

        if (country is not null && country.Lines is not null && country.Lines.Count > 0)
        {
            return new MaskResolutionResult
            {
                Mask = BuildMaskDefinition(country, MaskResolutionStatus.Exact),
                MaskResolutionStatus = MaskResolutionStatus.Exact,
                MaskVersion = _catalogVersion,
                MaskSource = _maskSource
            };
        }

        var fallbackCountry = country ?? BuildUnknownCountry(normalized);
        var warnings = new List<MaskResolutionWarning>
        {
            new("MASK_FALLBACK", "Fallback mask layout used for requested country", MaskResolutionStatus.Fallback)
        };

        return new MaskResolutionResult
        {
            Mask = BuildMaskDefinition(fallbackCountry, MaskResolutionStatus.Fallback),
            MaskResolutionStatus = MaskResolutionStatus.Fallback,
            MaskVersion = _catalogVersion,
            MaskSource = _maskSource,
            Warnings = warnings
        };
    }

    private CatalogCountry? ResolveCountry(string code)
    {
        if (_catalogByIso2.TryGetValue(code, out var iso2))
        {
            return iso2;
        }

        if (_catalogByIso3.TryGetValue(code, out var iso3))
        {
            return iso3;
        }

        return null;
    }

    private CountryMaskDefinition BuildMaskDefinition(CatalogCountry country, MaskResolutionStatus status)
    {
        var lines = status == MaskResolutionStatus.Exact && country.Lines is { Count: > 0 }
            ? country.Lines
            : _fallbackLayout;

        var clone = lines.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.ToList(),
            StringComparer.OrdinalIgnoreCase);

        return new CountryMaskDefinition
        {
            Country = country.Country,
            Iso3char = country.Iso3char,
            Iso2char = country.Iso2char,
            MaskVersion = _catalogVersion,
            MaskSource = _maskSource,
            Lines = clone,
            MaxLine = CountryMaskDefinition.DeriveMaxLine(clone)
        };
    }

    private static CatalogCountry BuildUnknownCountry(string iso2)
    {
        return new CatalogCountry
        {
            Country = iso2,
            Iso2char = iso2,
            Iso3char = "UNK"
        };
    }

    private sealed class CatalogRoot
    {
        [JsonPropertyName("catalog_version")]
        public required string CatalogVersion { get; init; }

        [JsonPropertyName("mask_source")]
        public required string MaskSource { get; init; }

        [JsonPropertyName("fallback_layout")]
        public required Dictionary<string, List<string>> FallbackLayout { get; init; }

        [JsonPropertyName("countries")]
        public required List<CatalogCountry> Countries { get; init; }
    }

    private sealed class CatalogCountry
    {
        public required string Country { get; init; }
        public required string Iso3char { get; init; }
        public required string Iso2char { get; init; }

        [JsonPropertyName("lines")]
        public Dictionary<string, List<string>>? Lines { get; init; }
    }
}
