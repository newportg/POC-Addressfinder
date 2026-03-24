namespace AddressFinder.FunctionApp.Domain.Models;

public sealed record MaskResolutionWarning(string WarningCode, string WarningMessage, MaskResolutionStatus MaskResolutionStatus);
