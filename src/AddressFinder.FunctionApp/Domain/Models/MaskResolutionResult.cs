namespace AddressFinder.FunctionApp.Domain.Models;

public sealed class MaskResolutionResult
{
    public required CountryMaskDefinition Mask { get; init; }
    public required MaskResolutionStatus MaskResolutionStatus { get; init; }
    public required string MaskVersion { get; init; }
    public required string MaskSource { get; init; }
    public List<MaskResolutionWarning> Warnings { get; init; } = [];
}
