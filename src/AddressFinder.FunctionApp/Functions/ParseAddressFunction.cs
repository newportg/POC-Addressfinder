using System.Net;
using AddressFinder.FunctionApp.Contracts;
using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.FunctionApp.Infrastructure.Telemetry;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AddressFinder.FunctionApp.Functions;

public class ParseAddressFunction(
    MaskResolutionService resolutionService,
    InputValidationService validationService,
    MaskResolutionTelemetry telemetry)
{
    [Function("ParseAddress")]
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
