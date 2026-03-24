using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.UnitTests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace AddressFinder.UnitTests;

public class MaskResolutionServiceTests
{
    [Fact]
    public void ShouldReturnExactMaskForSupportedCountry()
    {
        var provider = new FakeCatalogProvider();
        var sut = new MaskResolutionService(provider, new MaskContentSafetyValidator());

        var result = sut.ResolveByCountryCode("US");

        result.MaskResolutionStatus.Should().Be(MaskResolutionStatus.Exact);
        result.Mask.Country.Should().Be("United States");
    }

    [Fact]
    public void ShouldIncludePoBoxInParsedPayload_WhenInputContainsPoBox()
    {
        var provider = new FakeCatalogProvider();
        var sut = new MaskResolutionService(provider, new MaskContentSafetyValidator());
        var resolved = sut.ResolveByCountryCode("US");

        var payload = sut.BuildParsedAddressPayload("PO BOX 123, Seattle US", resolved, "req");

        var parsed = (Dictionary<string, string?>)payload.ParsedAddress;
        parsed["PostBox"].Should().Be("PO BOX");
    }

    [Fact]
    public void ShouldDefaultToGb_WhenParsedCountryHintIsMissing()
    {
        var provider = new FakeCatalogProvider();
        var sut = new MaskResolutionService(provider, new MaskContentSafetyValidator());

        sut.ResolveFromParsedCountryHint(null);

        provider.LastCountryCode.Should().Be("GB");
    }
}
