using AddressFinder.FunctionApp.Domain.Models;
using AddressFinder.FunctionApp.Infrastructure.Catalog;

namespace AddressFinder.FunctionApp.Domain.Services;

public sealed class MaskResolutionService(
    IEmbeddedMaskCatalogProvider catalogProvider,
    MaskContentSafetyValidator safetyValidator)
{
    public MaskResolutionResult ResolveByCountryCode(string countryCode)
    {
        var resolution = catalogProvider.Resolve(countryCode);
        EnsureSafety(resolution.Mask);
        return resolution;
    }

    public MaskResolutionResult ResolveFromParsedCountryHint(string? countryHint)
    {
        var resolution = catalogProvider.Resolve(countryHint ?? "US");
        EnsureSafety(resolution.Mask);
        return resolution;
    }

    public ParsedAddressWithMask BuildParsedAddressPayload(string originalInput, MaskResolutionResult resolution, string requestId)
    {
        var parsedAddress = new Dictionary<string, string?>
        {
            ["AddressLine1"] = originalInput,
            ["Country"] = resolution.Mask.Country,
            ["Iso2char"] = resolution.Mask.Iso2char,
            ["Iso3char"] = resolution.Mask.Iso3char,
            ["PostBox"] = resolution.Mask.SupportsPoBox && originalInput.Contains("PO BOX", StringComparison.OrdinalIgnoreCase)
                ? "PO BOX"
                : null
        };

        return new ParsedAddressWithMask
        {
            ParsedAddress = parsedAddress,
            Mask = resolution.Mask,
            MaskResolutionStatus = resolution.MaskResolutionStatus,
            MaskVersion = resolution.MaskVersion,
            MaskSource = resolution.MaskSource,
            RequestId = requestId,
            Warnings = resolution.Warnings
        };
    }

    private void EnsureSafety(CountryMaskDefinition def)
    {
        foreach (var line in def.Lines.Values)
        {
            if (!safetyValidator.IsSafe(line))
            {
                throw new InvalidOperationException("Unsafe mask component detected.");
            }
        }
    }
}
