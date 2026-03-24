using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.UnitTests.TestDoubles;
using FluentAssertions;

namespace AddressFinder.UnitTests;

public class FallbackResolutionTests
{
    [Fact]
    public void ShouldReportFallbackStatusWhenProviderReturnsFallback()
    {
        var provider = new FakeCatalogProvider
        {
            NextResult = new MaskResolutionResult
            {
                Mask = new FakeCatalogProvider().NextResult.Mask with
                {
                    Country = "Canada",
                    Iso2char = "CA",
                    Iso3char = "CAN"
                },
                MaskResolutionStatus = MaskResolutionStatus.Fallback,
                MaskVersion = "2026.03.23",
                MaskSource = "Loqate",
                Warnings = [new MaskResolutionWarning("MASK_FALLBACK", "fallback", MaskResolutionStatus.Fallback)]
            }
        };

        var sut = new MaskResolutionService(provider, new MaskContentSafetyValidator());
        var result = sut.ResolveByCountryCode("CA");

        result.MaskResolutionStatus.Should().Be(MaskResolutionStatus.Fallback);
        result.Mask.Iso2char.Should().Be("CA");
    }
}
