using AddressFinder.FunctionApp.Contracts;
using FluentAssertions;
using Xunit;

namespace AddressFinder.ContractTests;

public class ParseAddressCompatibilityContractTests
{
    [Fact]
    public void ParseResponse_ShouldPreserveLegacyParsedAddressFields()
    {
        var result = ParseAddressCompatibilityFixtures.CreateSample();
        var response = ParseAddressWithMaskResponse.FromResult(result);

        response.ParsedAddress.Should().BeOfType<Dictionary<string, string?>>();
        var parsed = (Dictionary<string, string?>)response.ParsedAddress;
        parsed.Should().ContainKey("AddressLine1");
        parsed.Should().ContainKey("Country");
        parsed.Should().ContainKey("Iso2char");
        parsed.Should().ContainKey("Iso3char");
    }
}
