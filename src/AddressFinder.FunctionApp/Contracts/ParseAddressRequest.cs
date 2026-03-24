using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace AddressFinder.FunctionApp.Contracts;

public sealed class ParseAddressRequest
{
	[JsonPropertyName("address_input")]
	[OpenApiProperty(Description = "Free-form address input as a UTF-8 string. Maximum length is 1000 characters.")]
	public string? AddressInput { get; init; }
}
