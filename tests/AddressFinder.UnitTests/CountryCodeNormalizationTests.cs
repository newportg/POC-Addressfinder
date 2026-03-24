using AddressFinder.FunctionApp.Domain.Services;
using FluentAssertions;
using Xunit;

namespace AddressFinder.UnitTests;

public class CountryCodeNormalizationTests
{
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
