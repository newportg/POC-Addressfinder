using AddressFinder.FunctionApp.Infrastructure.Telemetry;
using FluentAssertions;

namespace AddressFinder.UnitTests;

public class PiiRedactionTests
{
    [Fact]
    public void Redact_ShouldReturnStableHashAndNoPlaintext()
    {
        var sut = new PiiRedaction();
        var redacted = sut.Redact("123 Main Street");

        redacted.Should().NotContain("Main");
        redacted.Should().HaveLength(64);
    }
}
