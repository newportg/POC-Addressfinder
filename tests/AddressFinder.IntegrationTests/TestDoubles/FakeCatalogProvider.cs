using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Infrastructure.Catalog;

namespace AddressFinder.IntegrationTests.TestDoubles;

internal sealed class FakeCatalogProvider : IEmbeddedMaskCatalogProvider
{
    public IReadOnlyDictionary<string, CountryMaskDefinition> GetAll() =>
        new Dictionary<string, CountryMaskDefinition>();

    public MaskResolutionResult Resolve(string countryCode)
    {
        var isFallback = countryCode == "CA";
        return new MaskResolutionResult
        {
            Mask = new CountryMaskDefinition
            {
                Country = isFallback ? "Canada" : "United States",
                Iso2char = countryCode,
                Iso3char = isFallback ? "CAN" : "USA",
                SupportsPoBox = true,
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
