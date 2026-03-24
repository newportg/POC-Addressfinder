using System.Text.Json.Serialization;
using AddressFinder.FunctionApp.Domain.Models;

namespace AddressFinder.FunctionApp.Contracts;

public sealed class ParseAddressWithMaskResponse
{
    [JsonPropertyName("parsed_address")]
    public required object ParsedAddress { get; init; }

    [JsonPropertyName("mask")]
    public required MaskDto Mask { get; init; }

    [JsonPropertyName("mask_resolution_status")]
    public required string MaskResolutionStatus { get; init; }

    [JsonPropertyName("mask_version")]
    public required string MaskVersion { get; init; }

    [JsonPropertyName("mask_source")]
    public required string MaskSource { get; init; }

    [JsonPropertyName("request_id")]
    public required string RequestId { get; init; }

    [JsonPropertyName("warnings")]
    public List<WarningDto> Warnings { get; init; } = [];

    public static ParseAddressWithMaskResponse FromResult(ParsedAddressWithMask result)
    {
        return new ParseAddressWithMaskResponse
        {
            ParsedAddress = result.ParsedAddress,
            Mask = MaskDto.FromDomain(result.Mask),
            MaskResolutionStatus = result.MaskResolutionStatus.ToString().ToLowerInvariant(),
            MaskVersion = result.MaskVersion,
            MaskSource = result.MaskSource,
            RequestId = result.RequestId,
            Warnings = result.Warnings.Select(w => new WarningDto
            {
                WarningCode = w.WarningCode,
                WarningMessage = w.WarningMessage,
                MaskResolutionStatus = w.MaskResolutionStatus.ToString().ToLowerInvariant()
            }).ToList()
        };
    }
}
