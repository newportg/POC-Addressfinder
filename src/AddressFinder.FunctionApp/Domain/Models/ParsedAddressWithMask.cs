namespace AddressFinder.FunctionApp.Domain.Models;

public sealed class ParsedAddressWithMask
{
    public required object ParsedAddress { get; init; }
    public required CountryMaskDefinition Mask { get; init; }
    public required MaskResolutionStatus MaskResolutionStatus { get; init; }
    public required string MaskVersion { get; init; }
    public required string MaskSource { get; init; }
    public required string RequestId { get; init; }
    public List<MaskResolutionWarning> Warnings { get; init; } = [];
}
