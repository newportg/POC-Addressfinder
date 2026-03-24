using AddressFinder.FunctionApp.Domain.Services;
using FluentAssertions;
using Xunit;

namespace AddressFinder.UnitTests;

public class CountryCodeNormalizationTests
{
    [Fact]
    public void ValidateAddressInput_ShouldAcceptTypicalUkAddress()
    {
        var sut = new InputValidationService();

        var error = sut.ValidateAddressInput("68 westfield road, woking, surrey, gu22 9ng");

        error.Should().BeNull();
    }

    [Fact]
    public void ValidateAddressInput_ShouldRejectMissingAddressInput()
    {
        var sut = new InputValidationService();

        var error = sut.ValidateAddressInput(null);

        error.Should().Be("INVALID_INPUT");
    }

    [Theory]
    [InlineData("us", "US")]
    [InlineData("USA", "US")]
    [InlineData(" gbr ", "GB")]
    public void NormalizeCountryCode_ShouldNormalizeKnownInputs(string input, string expected)
    {
        var sut = new InputValidationService();

        var normalized = sut.NormalizeCountryCode(input);

        normalized.Should().Be(expected);
    }
}
