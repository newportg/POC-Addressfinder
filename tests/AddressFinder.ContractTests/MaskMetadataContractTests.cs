using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Models;
using FluentAssertions;
using Xunit;

namespace AddressFinder.ContractTests;

public class MaskMetadataContractTests
{
    [Fact]
    public void Responses_ShouldAlwaysExposeMaskVersionAndSource()
    {
        var sample = ParseAddressCompatibilityFixtures.CreateSample();
        var parseResponse = ParseAddressWithMaskResponse.FromResult(sample);

        var lookup = MaskLookupResponse.FromResult(new MaskResolutionResult
        {
            Mask = sample.Mask,
            MaskResolutionStatus = MaskResolutionStatus.Exact,
            MaskSource = sample.MaskSource,
            MaskVersion = sample.MaskVersion
        }, "id-1");

        parseResponse.MaskVersion.Should().Be(sample.MaskVersion);
        parseResponse.MaskSource.Should().Be(sample.MaskSource);
        lookup.MaskVersion.Should().Be(sample.MaskVersion);
        lookup.MaskSource.Should().Be(sample.MaskSource);
    }
}
