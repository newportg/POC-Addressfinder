using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Models;
using FluentAssertions;
using Xunit;

namespace AddressFinder.ContractTests;

public class GetAddressMaskContractTests
{
    [Fact]
    public void LookupResponse_ShouldContainMaskAndRequestId()
    {
        var result = new MaskResolutionResult
        {
            Mask = ParseAddressCompatibilityFixtures.CreateSample().Mask,
            MaskResolutionStatus = MaskResolutionStatus.Exact,
            MaskSource = "Loqate",
            MaskVersion = "2026.03.23"
        };

        var response = MaskLookupResponse.FromResult(result, "req-42");

        response.RequestId.Should().Be("req-42");
        response.Mask.Iso2char.Should().Be("US");
        response.Mask.DynamicLines.Should().ContainKey("Line2");
    }
}
