# Quickstart: Address Parsing API

## Prerequisites
- .NET 8 SDK
- Azure Functions Core Tools v4
- Azure CLI
- Bicep CLI (or Azure CLI with Bicep support)

## 1. Restore and Build
```powershell
dotnet restore
dotnet build
```

## 2. Run Tests First (Fail-First Gate)
```powershell
dotnet test tests/AddressFinder.ContractTests
dotnet test tests/AddressFinder.UnitTests
dotnet test tests/AddressFinder.IntegrationTests
```

## 3. Run Function Locally
```powershell
func start --csharp
```

## 4. Example Request
```http
POST http://localhost:7071/api/address/parse
Content-Type: application/json

{
  "address_input": "PO Box 123, 42 Downing Street, London SW1A 2AA, GB"
}
```

## 5. Expected Behavior
- Response follows OpenAPI contract in `contracts/address-parse.openapi.yaml`.
- PO Box is returned in `post_box_number`.
- Timestamps are RFC 3339 in UTC.
- `request_id` appears in both success telemetry and error payloads.

## 6. Deploy Infrastructure (Bicep)
```powershell
az deployment group create \
  --resource-group <rg-name> \
  --template-file src/AddressFinder.Infrastructure.Bicep/main.bicep \
  --parameters src/AddressFinder.Infrastructure.Bicep/parameters/dev.parameters.json
```

## 7. Telemetry Verification
- Confirm traces exist for endpoint invocation.
- Confirm metrics for latency, success rate, and cache hit ratio.
- Confirm logs do not contain raw address input.
