using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Infrastructure.Catalog;

namespace AddressFinder.UnitTests.TestDoubles;

internal sealed class FakeCatalogProvider : IEmbeddedMaskCatalogProvider
{
    public string? LastCountryCode { get; private set; }

    public MaskResolutionResult NextResult { get; set; } = new()
    {
        Mask = new CountryMaskDefinition
        {
            Country = "United States",
            Iso3char = "USA",
            Iso2char = "US",
            MaskSource = "Loqate",
            MaskVersion = "2026.03.23",
            Lines = new Dictionary<string, List<string>>
            {
                ["Line1"] = ["Organization"],
                ["Line2"] = ["Street"]
            },
            MaxLine = 2
        },
        MaskResolutionStatus = MaskResolutionStatus.Exact,
        MaskVersion = "2026.03.23",
        MaskSource = "Loqate"
    };

    public IReadOnlyDictionary<string, CountryMaskDefinition> GetAll() =>
        new Dictionary<string, CountryMaskDefinition> { ["US"] = NextResult.Mask };

    public MaskResolutionResult Resolve(string countryCode)
    {
        LastCountryCode = countryCode;
        return NextResult;
    }
}
