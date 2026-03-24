# Quick Validation: 002-country-display-mask

## Scope

- Parse endpoint returns mask payload and metadata
- Lookup endpoint returns exact/fallback mask behavior
- Validation errors include request identifier

## Execution Notes

Runtime validation could not be executed in this environment because no .NET SDK is installed.

## Static Validation Completed

- Added domain, contracts, services, and provider wiring for mask resolution
- Added deterministic fallback path preserving requested country metadata
- Added contract, unit, and integration test scaffolds for all user stories
- Added OpenAPI annotations for lookup endpoint

## Follow-up

Run locally after SDK installation:

1. `dotnet build src/AddressFinder.FunctionApp/AddressFinder.FunctionApp.csproj`
2. `dotnet test tests/AddressFinder.UnitTests/AddressFinder.UnitTests.csproj`
3. `dotnet test tests/AddressFinder.ContractTests/AddressFinder.ContractTests.csproj`
4. `dotnet test tests/AddressFinder.IntegrationTests/AddressFinder.IntegrationTests.csproj`
