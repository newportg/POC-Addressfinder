using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class MaskMetadataIntegrationTests
{
    [Fact]
    public void Metadata_ShouldIncludeDynamicLinesAndMaxLine()
    {
        var service = new MaskResolutionService(new FakeCatalogProvider(), new MaskContentSafetyValidator());
        var resolved = service.ResolveByCountryCode("US");
        var response = MaskLookupResponse.FromResult(resolved, "req");

        response.Mask.DynamicLines.Should().ContainKey("Line3");
        response.Mask.MaxLine.Should().Be(3);
    }
}
