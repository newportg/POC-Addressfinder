# Research: Address Parsing API

## Decision 1: Runtime and Hosting Model
- Decision: Use .NET 8 C# Azure Functions isolated worker.
- Rationale: Aligns with requested stack, supports modern dependency injection and OpenTelemetry wiring, and has clean separation of runtime concerns.
- Alternatives considered:
  - ASP.NET Core Web API: richer pipeline but heavier hosting model than needed for single endpoint.
  - Azure Functions in-process: legacy model with reduced future-forward support compared with isolated worker.

## Decision 2: API Contract Strategy
- Decision: Define OpenAPI-first contract in `/contracts/address-parse.openapi.yaml` and keep implementation aligned to that schema.
- Rationale: Enforces spec-driven delivery, enables contract tests, and documents ISO 20022-aligned response shape early.
- Alternatives considered:
  - Code-first annotations only: quick start but weaker governance and easier drift from spec.
  - Postman-only documentation: not sufficient for strict contract validation in CI.

## Decision 3: Address Schema Mapping
- Decision: Map response to ISO 20022 AddressComposer-oriented fields with `po_box` as a dedicated element.
- Rationale: User explicitly requires ISO 20022 alignment; separate PO Box avoids field ambiguity and helps downstream routing.
- Alternatives considered:
  - Flattened non-standard schema (`street_address`, `city`, `state_code`): simpler but non-compliant with requested standard.
  - Fully nested ISO object graph: more expressive but adds unnecessary complexity for MVP.

## Decision 4: Caching and Persistence
- Decision: Persist parsed responses in encrypted storage with SHA256(normalized_input) cache key and 90-day TTL.
- Rationale: Meets explicit repeated-query performance requirement while preserving privacy and avoiding raw address index keys.
- Alternatives considered:
  - No persistence: violates repeated-response requirement.
  - In-memory cache only: insufficient across function instances and cold starts.

## Decision 5: Observability Stack
- Decision: Use OpenTelemetry for traces/metrics/log correlation with Azure Monitor exporter.
- Rationale: Standardized telemetry model, supports request correlation and latency/error SLO tracking.
- Alternatives considered:
  - Application Insights SDK only (without OTel): workable but less portable and less aligned with requested stack.
  - Custom logging only: insufficient for distributed tracing and robust operations signals.

## Decision 6: Infrastructure as Code
- Decision: Provision Azure resources via Bicep modules for Function App, storage/cache, key management, and monitoring.
- Rationale: Reproducible environments and constitution-compliant dependency documentation.
- Alternatives considered:
  - Portal/manual setup: fast once, poor repeatability.
  - Terraform: valid option but user requested Bicep.
