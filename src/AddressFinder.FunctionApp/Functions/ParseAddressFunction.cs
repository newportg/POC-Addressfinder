using System.Net;
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
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ParseAddressRequest), Required = true, Description = "Address parse request payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ParseAddressWithMaskResponse))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ParseErrorResponse))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(ParseErrorResponse))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "address/parse")]
        HttpRequestData req)
    {
        var requestId = Guid.NewGuid().ToString("N");
        var payload = await req.ReadFromJsonAsync<ParseAddressRequest>() ?? new ParseAddressRequest(string.Empty);

        var inputError = validationService.ValidateAddressInput(payload.AddressInput);
        if (inputError is not null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new ParseErrorResponse(inputError, "Invalid address input", requestId));
            return bad;
        }

        var countryHint = validationService.InferCountryCode(payload.AddressInput) ?? "US";
        var normalizedCountry = validationService.NormalizeCountryCode(countryHint) ?? "US";
        var result = resolutionService.ResolveByCountryCode(normalizedCountry);
        telemetry.TrackMaskResolution(requestId, result.MaskResolutionStatus, result.MaskVersion, result.MaskSource);

        var parsedPayload = resolutionService.BuildParsedAddressPayload(payload.AddressInput, result, requestId);

        var ok = req.CreateResponse(HttpStatusCode.OK);
        await ok.WriteAsJsonAsync(ParseAddressWithMaskResponse.FromResult(parsedPayload));
        return ok;
    }
}
