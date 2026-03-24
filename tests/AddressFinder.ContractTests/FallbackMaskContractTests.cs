using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Models;
using FluentAssertions;

namespace AddressFinder.ContractTests;

public class FallbackMaskContractTests
{
    [Fact]
    public void LookupFallback_ShouldKeepCountryMetadataAndFallbackStatus()
    {
        var sampleMask = ParseAddressCompatibilityFixtures.CreateSample().Mask with
        {
            Country = "Canada",
            Iso2char = "CA",
            Iso3char = "CAN"
        };

        var result = new MaskResolutionResult
        {
            Mask = sampleMask,
            MaskResolutionStatus = MaskResolutionStatus.Fallback,
            MaskSource = "Loqate",
            MaskVersion = "2026.03.23",
            Warnings = [new MaskResolutionWarning("MASK_FALLBACK", "fallback", MaskResolutionStatus.Fallback)]
        };

        var response = MaskLookupResponse.FromResult(result, "r");

        response.MaskResolutionStatus.Should().Be("fallback");
        response.Mask.Iso2char.Should().Be("CA");
        response.Warnings.Should().NotBeEmpty();
    }
}
