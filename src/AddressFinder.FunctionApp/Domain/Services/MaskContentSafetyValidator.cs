namespace AddressFinder.FunctionApp.Domain.Services;

public sealed class MaskContentSafetyValidator
{
    private static readonly string[] BlockedTokens = ["<script", "${", "{{", "}}", "<%", "%>"];

    public bool IsSafe(IEnumerable<string> components)
    {
        foreach (var component in components)
        {
            var value = component?.ToLowerInvariant() ?? string.Empty;
            if (BlockedTokens.Any(value.Contains))
            {
                return false;
            }
        }

        return true;
    }
}
