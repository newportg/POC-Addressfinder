using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class FallbackMaskIntegrationTests
{
    [Fact]
    public void Fallback_ShouldPreserveCountryMetadata()
    {
        var service = new MaskResolutionService(new FakeCatalogProvider(), new MaskContentSafetyValidator());
        var fallback = service.ResolveByCountryCode("CA");

        fallback.Mask.Country.Should().Be("Canada");
        fallback.Mask.Iso3char.Should().Be("CAN");
    }
}
