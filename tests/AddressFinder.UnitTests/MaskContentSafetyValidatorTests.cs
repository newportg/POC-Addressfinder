using AddressFinder.FunctionApp.Domain.Services;
using FluentAssertions;

namespace AddressFinder.UnitTests;

public class MaskContentSafetyValidatorTests
{
    [Fact]
    public void IsSafe_ShouldRejectExecutableTokens()
    {
        var sut = new MaskContentSafetyValidator();

        sut.IsSafe(["Organization", "{{danger}}"])
            .Should()
            .BeFalse();
    }

    [Fact]
    public void IsSafe_ShouldAllowDeclarativeComponentsOnly()
    {
        var sut = new MaskContentSafetyValidator();

        sut.IsSafe(["Organization", "PostalCode", "Locality"])
            .Should()
            .BeTrue();
    }
}
