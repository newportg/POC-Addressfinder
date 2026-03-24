using AddressFinder.FunctionApp.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AddressFinder.FunctionApp.Infrastructure.Telemetry;

public sealed class MaskResolutionTelemetry(ILogger<MaskResolutionTelemetry> logger)
{
    public void TrackMaskResolution(string requestId, MaskResolutionStatus status, string maskVersion, string maskSource)
    {
        logger.LogInformation(
            "mask_resolution request_id={RequestId} status={Status} mask_version={MaskVersion} mask_source={MaskSource}",
            requestId,
            status.ToString().ToLowerInvariant(),
            maskVersion,
            maskSource);
    }
}
