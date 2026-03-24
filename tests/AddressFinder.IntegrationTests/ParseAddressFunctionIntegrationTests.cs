using System.Net;
using System.Text.Json;
using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.FunctionApp.Functions;
using AddressFinder.FunctionApp.Infrastructure.Telemetry;
using AddressFinder.IntegrationTests.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class ParseAddressFunctionIntegrationTests
{
    [Fact]
    public async Task Run_ShouldDefaultToGb_WhenUkAddressHasNoCountryDesignation()
    {
        var catalogProvider = new FakeCatalogProvider();
        var function = new ParseAddressFunction(
            new MaskResolutionService(catalogProvider, new MaskContentSafetyValidator()),
            new InputValidationService(),
            new MaskResolutionTelemetry(NullLogger<MaskResolutionTelemetry>.Instance));

        var requestBody = JsonSerializer.Serialize(new ParseAddressRequest
        {
            AddressInput = "68 westfield road, woking, surrey, gu22 9ng"
        });
        var request = new FakeHttpRequestData(new FakeFunctionContext(), requestBody);

        var response = await function.Run(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        catalogProvider.LastCountryCode.Should().Be("GB");

        response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(response.Body);
        document.RootElement.GetProperty("mask").GetProperty("Iso2char").GetString().Should().Be("GB");
        document.RootElement.GetProperty("mask").GetProperty("Country").GetString().Should().Be("United Kingdom");
    }

    [Fact]
    public async Task Run_ShouldAcceptCamelCaseAddressInputProperty()
    {
        var catalogProvider = new FakeCatalogProvider();
        var function = new ParseAddressFunction(
            new MaskResolutionService(catalogProvider, new MaskContentSafetyValidator()),
            new InputValidationService(),
            new MaskResolutionTelemetry(NullLogger<MaskResolutionTelemetry>.Instance));

        const string requestBody = """
        {
          "addressInput": "68 westfield road, woking, surrey, gu22 9ng, UK"
        }
        """;
        var request = new FakeHttpRequestData(new FakeFunctionContext(), requestBody);

        var response = await function.Run(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        catalogProvider.LastCountryCode.Should().Be("GB");
    }
}