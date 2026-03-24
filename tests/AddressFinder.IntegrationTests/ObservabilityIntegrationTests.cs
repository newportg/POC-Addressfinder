using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;

namespace AddressFinder.IntegrationTests;

public class ObservabilityIntegrationTests
{
    [Fact]
    public void Resolution_ShouldCarryStatusAndVersionForTelemetry()
    {
        var service = new MaskResolutionService(new FakeCatalogProvider(), new MaskContentSafetyValidator());
        var result = service.ResolveByCountryCode("US");

        result.MaskVersion.Should().NotBeNullOrWhiteSpace();
        result.MaskResolutionStatus.ToString().Should().NotBeNullOrWhiteSpace();
    }
}
