using AddressFinder.FunctionApp.Domain.Models;

namespace AddressFinder.ContractTests;

public static class ParseAddressCompatibilityFixtures
{
    public static ParsedAddressWithMask CreateSample()
    {
        return new ParsedAddressWithMask
        {
            ParsedAddress = new Dictionary<string, string?>
            {
                ["AddressLine1"] = "1 Main St",
                ["Country"] = "United States",
                ["Iso2char"] = "US",
                ["Iso3char"] = "USA"
            },
            Mask = new CountryMaskDefinition
            {
                Country = "United States",
                Iso2char = "US",
                Iso3char = "USA",
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
            MaskResolutionStatus = MaskResolutionStatus.Exact,
            MaskSource = "Loqate",
            MaskVersion = "2026.03.23",
            RequestId = "req-1"
        };
    }
}
