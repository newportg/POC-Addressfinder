using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Infrastructure.Catalog;

namespace AddressFinder.IntegrationTests.TestDoubles;

internal sealed class FakeCatalogProvider : IEmbeddedMaskCatalogProvider
{
    public string? LastCountryCode { get; private set; }

    public IReadOnlyDictionary<string, CountryMaskDefinition> GetAll() =>
        new Dictionary<string, CountryMaskDefinition>();

    public MaskResolutionResult Resolve(string countryCode)
    {
        LastCountryCode = countryCode;
        var isFallback = countryCode == "CA";
        var country = countryCode switch
        {
            "GB" => "United Kingdom",
            "CA" => "Canada",
            _ => "United States"
        };

        var iso3 = countryCode switch
        {
            "GB" => "GBR",
            "CA" => "CAN",
            _ => "USA"
        };

        return new MaskResolutionResult
        {
            Mask = new CountryMaskDefinition
            {
                Country = country,
                Iso2char = countryCode,
                Iso3char = iso3,
                MaskSource = "Loqate",
                MaskVersion = "2026.03.23",
                Lines = new Dictionary<string, List<string>>
                {
                    ["Line1"] = ["Organization"],
                    ["Line2"] = ["Street"],
                    ["Line3"] = ["PostalCode", "Locality"]
                },
                MaxLine = 3
            },
            MaskResolutionStatus = isFallback ? MaskResolutionStatus.Fallback : MaskResolutionStatus.Exact,
            MaskVersion = "2026.03.23",
            MaskSource = "Loqate"
        };
    }
}
