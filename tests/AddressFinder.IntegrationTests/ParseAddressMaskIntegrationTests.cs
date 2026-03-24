using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class ParseAddressMaskIntegrationTests
{
    [Fact]
    public void ParseFlow_ShouldMapUsAndGbToMaskPayloads()
    {
        var service = new MaskResolutionService(new FakeCatalogProvider(), new MaskContentSafetyValidator());

        var us = service.ResolveByCountryCode("US");
        var gb = service.ResolveByCountryCode("GB");

        us.MaskResolutionStatus.Should().Be(AddressFinder.FunctionApp.Domain.Models.MaskResolutionStatus.Exact);
        gb.MaskResolutionStatus.Should().Be(AddressFinder.FunctionApp.Domain.Models.MaskResolutionStatus.Exact);
    }
}
