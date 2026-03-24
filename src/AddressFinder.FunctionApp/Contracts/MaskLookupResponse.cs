using System.Text.Json;
using System.Text.Json.Serialization;
using AddressFinder.FunctionApp.Domain.Models;

namespace AddressFinder.FunctionApp.Contracts;

public sealed class MaskLookupResponse
{
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

    public static MaskLookupResponse FromResult(MaskResolutionResult result, string requestId)
    {
        return new MaskLookupResponse
        {
            Mask = MaskDto.FromDomain(result.Mask),
            MaskResolutionStatus = result.MaskResolutionStatus.ToString().ToLowerInvariant(),
            MaskVersion = result.MaskVersion,
            MaskSource = result.MaskSource,
            RequestId = requestId,
            Warnings = result.Warnings.Select(w => new WarningDto
            {
                WarningCode = w.WarningCode,
                WarningMessage = w.WarningMessage,
                MaskResolutionStatus = w.MaskResolutionStatus.ToString().ToLowerInvariant()
            }).ToList()
        };
    }
}

public sealed class MaskDto
{
    [JsonPropertyName("Country")]
    public required string Country { get; init; }

    [JsonPropertyName("Iso3char")]
    public required string Iso3char { get; init; }

    [JsonPropertyName("Iso2char")]
    public required string Iso2char { get; init; }

    [JsonPropertyName("max_line")]
    public required int MaxLine { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> DynamicLines { get; init; } = [];

    public static MaskDto FromDomain(CountryMaskDefinition definition)
    {
        var extensionData = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in definition.Lines.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            extensionData[kvp.Key] = JsonSerializer.SerializeToElement(kvp.Value);
        }

        return new MaskDto
        {
            Country = definition.Country,
            Iso3char = definition.Iso3char,
            Iso2char = definition.Iso2char,
            MaxLine = definition.MaxLine,
            DynamicLines = extensionData
        };
    }
}

public sealed class WarningDto
{
    [JsonPropertyName("warning_code")]
    public required string WarningCode { get; init; }

    [JsonPropertyName("warning_message")]
    public required string WarningMessage { get; init; }

    [JsonPropertyName("mask_resolution_status")]
    public required string MaskResolutionStatus { get; init; }
}
