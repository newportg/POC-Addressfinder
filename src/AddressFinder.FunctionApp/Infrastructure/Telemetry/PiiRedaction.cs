using System.Security.Cryptography;
using System.Text;

namespace AddressFinder.FunctionApp.Infrastructure.Telemetry;

public sealed class PiiRedaction
{
    public string Redact(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value.Trim()));
        return Convert.ToHexString(bytes);
    }
}
