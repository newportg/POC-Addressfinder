# Implementation Plan: Address Parsing API

**Branch**: `001-address-api` | **Date**: 2026-03-23 | **Spec**: `/specs/001-address-api/spec.md`
**Input**: Feature specification from `/specs/001-address-api/spec.md`

## Summary

Build a C# Azure Functions HTTP API that accepts a free-form address string and returns an ISO 20022-aligned structured JSON payload, with PO Box extraction, validation normalization, and RFC 3339 timestamps. The solution includes OpenAPI contract publishing, OpenTelemetry-based traces/metrics/log correlation, encrypted persistent cache for repeated requests, and Bicep infrastructure definitions for deployable cloud resources.

## Technical Context

**Language/Version**: C# 12 on .NET 8 (Azure Functions isolated worker)  
**Primary Dependencies**: Azure Functions Worker SDK, Azure Functions OpenAPI Extension, OpenTelemetry.Extensions.Hosting, OpenTelemetry.Exporter.AzureMonitor, System.Text.Json  
**Storage**: Azure Cosmos DB (or Azure Table Storage fallback) for encrypted parsed-address cache, 90-day TTL  
**Testing**: xUnit, FluentAssertions, Microsoft.Azure.Functions.Worker.Extensions.Tests (or equivalent host testing), contract tests against OpenAPI document  
**Target Platform**: Azure Functions on Linux Consumption or Premium plan
**Project Type**: serverless web-service  
**Performance Goals**: p95 <= 500ms uncached; cached p95 <= 100ms; cache hit rate >= 30%  
**Constraints**: input length <= 1000 chars; no raw PII in logs; AES-256 at-rest encryption; request-id in all failures  
**Scale/Scope**: 1000 concurrent requests, initial country support US/UK/CA/DE/FR/JP

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Spec-Driven Delivery: PASS. Plan is based on approved `spec.md` with P1-P3 stories, scenarios, edge cases, and measurable criteria SC-001..SC-009.
- Data Minimization and Privacy: PASS. Sensitive data explicitly classified as PII, with redacted logs, encrypted cache, SHA256 keying, and 90-day retention.
- Test-First Quality Gates: PASS. Unit, integration, and contract test suites are defined to fail first per story before implementation completion.
- Observability and Explainability: PASS. OpenTelemetry traces/metrics/log correlation and request_id-based error diagnostics are mandatory outputs.
- Simplicity and Reversibility: PASS. Single Azure Functions service with explicit rollback path (disable cache writes, retain parser-only behavior) and no schema-breaking API version jump.

## Phase 0: Research Output

Research decisions are captured in `/specs/001-address-api/research.md`.
All prior clarifications are resolved; no remaining NEEDS CLARIFICATION markers.

## Phase 1: Design Output

- Data model: `/specs/001-address-api/data-model.md`
- API contract: `/specs/001-address-api/contracts/address-parse.openapi.yaml`
- Quickstart: `/specs/001-address-api/quickstart.md`

## Project Structure

### Documentation (this feature)

```text
specs/001-address-api/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── address-parse.openapi.yaml
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── AddressFinder.FunctionApp/
│   ├── Functions/
│   │   └── ParseAddressFunction.cs
│   ├── Domain/
│   │   ├── Models/
│   │   └── Services/
│   ├── Infrastructure/
│   │   ├── Cache/
│   │   ├── Encryption/
│   │   └── Telemetry/
│   ├── Contracts/
│   │   ├── Requests/
│   │   └── Responses/
│   ├── Program.cs
│   └── host.json
├── AddressFinder.Infrastructure.Bicep/
│   ├── main.bicep
│   ├── modules/
│   └── parameters/
└── AddressFinder.sln

tests/
├── AddressFinder.UnitTests/
├── AddressFinder.IntegrationTests/
└── AddressFinder.ContractTests/
```

**Structure Decision**: Single service repository with one Function App runtime project, one Bicep infra project, and three test projects for unit/integration/contract coverage.

## Test-First Strategy (Per User Story)

- US1 Parse Address:
  - Contract tests: POST `/api/address/parse` response schema and required ISO 20022 fields.
  - Unit tests: tokenization, component extraction, PO Box parsing.
  - Integration tests: function host + storage + parser wiring.
- US2 Standardization:
  - Unit tests: abbreviation normalization, country/subdivision normalization.
  - Integration tests: normalization pipeline in request flow.
- US3 Timestamps and Auditability:
  - Unit tests: RFC 3339 formatting and timezone handling.
  - Integration tests: response includes request_id and timestamp bounds.

## Rollout, Migration, and Rollback

- Rollout: deploy behind environment slot; warm up and run smoke tests before swap.
- Migration: create cache container/table with TTL and encryption key references before enabling cache writes.
- Rollback: swap slot back; disable cache feature flag; continue parser-only operation without persistent cache dependency.

## Re-Check Constitution After Design

- Spec-Driven Delivery: PASS (data model and contract map directly to FR/SC IDs).
- Data Minimization and Privacy: PASS (no IP/device metadata, encrypted storage only, redacted error payloads).
- Test-First Quality Gates: PASS (quickstart defines contract-first and fail-first sequence).
- Observability and Explainability: PASS (OpenTelemetry + request correlation + explicit error model).
- Simplicity and Reversibility: PASS (single function endpoint, one persistence concern, feature-flagged cache).

## Complexity Tracking

No constitutional violations requiring exception.
