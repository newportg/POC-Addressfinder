using System.Text.Json.Serialization;

namespace AddressFinder.FunctionApp.Contracts;

public sealed class ParseErrorResponse(string errorCode, string message, string requestId)
{
    [JsonPropertyName("error_code")]
    public string ErrorCode { get; } = errorCode;

    [JsonPropertyName("message")]
    public string Message { get; } = message;

    [JsonPropertyName("request_id")]
    public string RequestId { get; } = requestId;
}
