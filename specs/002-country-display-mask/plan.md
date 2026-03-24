# Implementation Plan: Country Display Masks For Parsed Addresses

**Branch**: `002-country-display-mask` | **Date**: 2026-03-23 | **Spec**: `specs/002-country-display-mask/spec.md`
**Input**: Feature specification from `/specs/002-country-display-mask/spec.md`

## Summary

Extend the address parsing API to include country-specific mask metadata in parse responses and add a standalone mask lookup endpoint by country code. Mask definitions are delivered from a versioned embedded JSON catalog derived from Loqate reference data. The API returns a structured `mask` object with country identity plus dynamic `Line1..LineN` component arrays and `max_line` metadata, and deterministic fallback behavior for missing country-specific masks.

## Technical Context

**Language/Version**: C# 12 on .NET 8 (Azure Functions isolated worker)  
**Primary Dependencies**: Azure Functions Worker SDK, Azure Functions OpenAPI Extension, OpenTelemetry.Extensions.Hosting, OpenTelemetry.Exporter.AzureMonitor, System.Text.Json  
**Storage**: Embedded JSON mask catalog (read-only), existing encrypted cache for parse responses where applicable  
**Testing**: xUnit, FluentAssertions, contract tests against generated OpenAPI, integration tests with Functions host  
**Target Platform**: Azure Functions on Linux (Consumption/Premium)
**Project Type**: serverless web-service  
**Performance Goals**: p95 additional latency from mask resolution <= 50ms; fallback lookup always returns within parse SLO budget  
**Constraints**: input length <= 1000 chars; no raw address PII in logs; deterministic fallback; structured non-executable mask output  
**Scale/Scope**: global country mask catalog, two endpoints (`POST /api/address/parse`, `GET /api/address/mask/{countryCode}`)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Spec-Driven Delivery: Plan MUST reference an approved spec with prioritized stories,
  acceptance scenarios, edge cases, and measurable outcomes.
- Data Minimization and Privacy: Plan MUST identify sensitive data involved, minimum
  required collection, retention approach, and redaction strategy for logs/tests.
- Test-First Quality Gates: Plan MUST define fail-first test strategy per user story,
  including unit and integration test scope.
- Observability and Explainability: Plan MUST define structured diagnostics,
  user-visible failure messaging, and troubleshooting signals.
- Simplicity and Reversibility: Plan MUST justify complexity, and any
  backward-incompatible change MUST include migration and rollback steps.

Constitution gate result: PASS

## Project Structure

### Documentation (this feature)

```text
specs/002-country-display-mask/
├── plan.md
├── spec.md
└── tasks.md
```

### Source Code (repository root)
```text
src/
├── AddressFinder.FunctionApp/
│   ├── Functions/
│   │   ├── ParseAddressFunction.cs
│   │   └── GetAddressMaskFunction.cs
│   ├── Domain/
│   │   ├── Models/
│   │   │   ├── CountryMaskDefinition.cs
│   │   │   └── ParsedAddressWithMask.cs
│   │   └── Services/
│   │       └── MaskResolutionService.cs
│   ├── Infrastructure/
│   │   ├── Catalog/
│   │   │   ├── country-mask-catalog.json
│   │   │   └── EmbeddedMaskCatalogProvider.cs
│   │   └── Telemetry/
│   │       └── MaskResolutionTelemetry.cs
│   ├── Contracts/
│   │   └── MaskLookupResponse.cs
│   └── Program.cs

tests/
├── AddressFinder.ContractTests/
├── AddressFinder.IntegrationTests/
└── AddressFinder.UnitTests/
```

**Structure Decision**: Single Azure Functions service with separated domain/infrastructure layers and dedicated test projects for contract, integration, and unit validation.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitutional exceptions required.
