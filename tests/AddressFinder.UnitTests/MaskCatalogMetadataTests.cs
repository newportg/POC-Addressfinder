using AddressFinder.FunctionApp.Domain.Models;
using FluentAssertions;

namespace AddressFinder.UnitTests;

public class MaskCatalogMetadataTests
{
    [Fact]
    public void DeriveMaxLine_ShouldUseHighestLineNumber()
    {
        var lines = new Dictionary<string, List<string>>
        {
            ["Line1"] = ["A"],
            ["Line7"] = ["B"],
            ["Line4"] = ["C"]
        };

        var maxLine = CountryMaskDefinition.DeriveMaxLine(lines);

        maxLine.Should().Be(7);
    }
}
