using AddressFinder.FunctionApp.Contracts;
using FluentAssertions;
using Xunit;

namespace AddressFinder.ContractTests;

public class ParseAddressMaskContractTests
{
    [Fact]
    public void ParseResponse_ShouldIncludeMaskObjectAndMetadata()
    {
        var result = ParseAddressCompatibilityFixtures.CreateSample();

        var response = ParseAddressWithMaskResponse.FromResult(result);

        response.Mask.Country.Should().Be("United States");
        response.Mask.DynamicLines.Should().ContainKey("Line1");
        response.MaskResolutionStatus.Should().Be("exact");
        response.MaskVersion.Should().NotBeNullOrWhiteSpace();
        response.MaskSource.Should().NotBeNullOrWhiteSpace();
    }
}
