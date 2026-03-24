using System.Net;
using System.Text;
using System.Text.Json;
using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.FunctionApp.Infrastructure.Telemetry;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace AddressFinder.FunctionApp.Functions;

public class ParseAddressFunction(
    MaskResolutionService resolutionService,
    InputValidationService validationService,
    MaskResolutionTelemetry telemetry)
{
    [Function("ParseAddress")]
    [OpenApiOperation(operationId: "ParseAddress", tags: ["Address"])]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ParseAddressRequest), Required = true, Description = "Address parse request payload", Example = typeof(ParseAddressRequestExample))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ParseAddressWithMaskResponse))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ParseErrorResponse))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ParseErrorResponse))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "address/parse")]
        HttpRequestData req)
    {
        var requestId = Guid.NewGuid().ToString("N");
        string rawBody;
        using (var reader = new StreamReader(req.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
        {
            rawBody = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(rawBody))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new ParseErrorResponse("MISSING_BODY", "Request body is required and must contain 'address_input' or 'addressInput'.", requestId));
            return bad;
        }

        JsonDocument payload;

        try
        {
            payload = JsonDocument.Parse(rawBody);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new ParseErrorResponse("MALFORMED_JSON", "Request body must be valid JSON containing 'address_input' or 'addressInput'.", requestId));
            return bad;
        }

        using (payload)
        {
            if (payload.RootElement.ValueKind is not JsonValueKind.Object)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteAsJsonAsync(new ParseErrorResponse("MALFORMED_JSON", "Request body must be a JSON object.", requestId));
                return bad;
            }

            var hasSnake = payload.RootElement.TryGetProperty("address_input", out var snake);
            var hasCamel = payload.RootElement.TryGetProperty("addressInput", out var camel);

            string? addressInput = null;
            if (hasSnake && snake.ValueKind == JsonValueKind.String)
            {
                addressInput = snake.GetString();
            }
            else if (hasCamel && camel.ValueKind == JsonValueKind.String)
            {
                addressInput = camel.GetString();
            }

            if (addressInput is null)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteAsJsonAsync(new ParseErrorResponse("MISSING_ADDRESS_INPUT", "Request body must include 'address_input' or 'addressInput'.", requestId));
                return bad;
            }

            var inputError = validationService.ValidateAddressInput(addressInput);
            if (inputError is not null)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteAsJsonAsync(new ParseErrorResponse(inputError, "Invalid address input", requestId));
                return bad;
            }

            var countryHint = validationService.InferCountryCode(addressInput);
            var normalizedCountry = validationService.NormalizeCountryCode(countryHint) ?? "GB";
            var result = resolutionService.ResolveByCountryCode(normalizedCountry);
            telemetry.TrackMaskResolution(requestId, result.MaskResolutionStatus, result.MaskVersion, result.MaskSource);

            var parsedPayload = resolutionService.BuildParsedAddressPayload(addressInput, result, requestId);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(ParseAddressWithMaskResponse.FromResult(parsedPayload));
            return ok;
        }
    }
}
