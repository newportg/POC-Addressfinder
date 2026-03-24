using System.Text.Json.Serialization;

namespace AddressFinder.FunctionApp.Contracts;

public sealed record ParseAddressRequest(
	[property: JsonPropertyName("address_input")] string AddressInput);
