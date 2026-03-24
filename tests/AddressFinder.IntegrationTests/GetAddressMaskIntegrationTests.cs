using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class GetAddressMaskIntegrationTests
{
    [Fact]
    public void Lookup_ShouldReturnFallbackForValidButMissingCountry()
    {
        var service = new MaskResolutionService(new FakeCatalogProvider(), new MaskContentSafetyValidator());

        var fallback = service.ResolveByCountryCode("CA");

        fallback.MaskResolutionStatus.ToString().ToLowerInvariant().Should().Be("fallback");
    }
}
