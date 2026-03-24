using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace AddressFinder.FunctionApp.Contracts;

public sealed class ParseAddressRequestExample : OpenApiExample<ParseAddressRequest>
{
    public override IOpenApiExample<ParseAddressRequest> Build(NamingStrategy namingStrategy)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "uk_address",
            new ParseAddressRequest
            {
                AddressInput = "68 westfield road, woking, surrey, gu22 9ng"
            },
            namingStrategy));

        return this;
    }
}