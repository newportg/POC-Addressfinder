# Tasks: Country Display Masks For Parsed Addresses

**Input**: Design documents from `/specs/002-country-display-mask/`  
**Prerequisites**: `plan.md` (required), `spec.md` (required)

**Tests**: Tests are required for each user story and are sequenced to fail before implementation.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish project skeleton and baseline tooling for the feature.

- [X] T001 Create function app folder structure in `src/AddressFinder.FunctionApp/`
- [X] T002 Create test project folders in `tests/AddressFinder.ContractTests/`, `tests/AddressFinder.IntegrationTests/`, and `tests/AddressFinder.UnitTests/`
- [X] T003 [P] Create base domain model files `src/AddressFinder.FunctionApp/Domain/Models/CountryMaskDefinition.cs` and `src/AddressFinder.FunctionApp/Domain/Models/ParsedAddressWithMask.cs`
- [X] T004 [P] Create base service and provider files `src/AddressFinder.FunctionApp/Domain/Services/MaskResolutionService.cs` and `src/AddressFinder.FunctionApp/Infrastructure/Catalog/EmbeddedMaskCatalogProvider.cs`
- [X] T005 [P] Create contract files `src/AddressFinder.FunctionApp/Contracts/MaskLookupResponse.cs` and `src/AddressFinder.FunctionApp/Contracts/ParseAddressWithMaskResponse.cs`
- [X] T006 [P] Create compatibility contract fixture for existing parse schema in `tests/AddressFinder.ContractTests/ParseAddressCompatibilityFixtures.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Complete cross-story prerequisites required before story implementation.

**CRITICAL**: No user story implementation starts before this phase is done.

- [X] T007 Implement catalog config binding and startup load in `src/AddressFinder.FunctionApp/Program.cs`
- [X] T008 Add embedded mask catalog JSON and schema version metadata in `src/AddressFinder.FunctionApp/Infrastructure/Catalog/country-mask-catalog.json`
- [X] T009 Implement catalog parsing and in-process caching in `src/AddressFinder.FunctionApp/Infrastructure/Catalog/EmbeddedMaskCatalogProvider.cs`
- [X] T010 Implement shared mask resolution status enum and warning model in `src/AddressFinder.FunctionApp/Domain/Models/MaskResolutionStatus.cs` and `src/AddressFinder.FunctionApp/Domain/Models/MaskResolutionWarning.cs`
- [X] T011 Implement shared input validation helper for country codes and parse input limits in `src/AddressFinder.FunctionApp/Domain/Services/InputValidationService.cs`
- [X] T012 Implement structured telemetry helpers for mask source/version/fallback signals in `src/AddressFinder.FunctionApp/Infrastructure/Telemetry/MaskResolutionTelemetry.cs`
- [X] T013 Add shared redaction utility for logs/errors in `src/AddressFinder.FunctionApp/Infrastructure/Telemetry/PiiRedaction.cs`
- [X] T014 Implement mask content safety validator to block executable syntax in `src/AddressFinder.FunctionApp/Domain/Services/MaskContentSafetyValidator.cs`

**Checkpoint**: Foundation ready; user stories can be implemented.

---

## Phase 3: User Story 1 - Return Country Display Mask With Parse Result (Priority: P1) 🎯 MVP

**Goal**: Add country-specific `mask` object to successful parse responses.

**Independent Test**: Call `POST /api/address/parse` with US/GB/PO Box samples and verify mask object appears with correct country metadata and line arrays.

### Tests for User Story 1 (Fail First)

- [X] T015 [P] [US1] Add contract test for parse response mask schema in `tests/AddressFinder.ContractTests/ParseAddressMaskContractTests.cs`
- [X] T016 [P] [US1] Add integration test for US/GB parse mask mapping in `tests/AddressFinder.IntegrationTests/ParseAddressMaskIntegrationTests.cs`
- [X] T017 [P] [US1] Add unit tests for PO Box-aware mask selection in `tests/AddressFinder.UnitTests/MaskResolutionServiceTests.cs`
- [X] T018 [P] [US1] Add compatibility regression contract test for FR-007 legacy parse fields in `tests/AddressFinder.ContractTests/ParseAddressCompatibilityContractTests.cs`

### Implementation for User Story 1

- [X] T019 [US1] Implement mask augmentation in parse response model mapping in `src/AddressFinder.FunctionApp/Domain/Services/MaskResolutionService.cs`
- [X] T020 [US1] Update parse function to include mask payload fields in `src/AddressFinder.FunctionApp/Functions/ParseAddressFunction.cs`
- [X] T021 [US1] Add parse response contract updates for `mask`, `mask_source`, `mask_version`, and `mask_resolution_status` in `src/AddressFinder.FunctionApp/Contracts/ParseAddressWithMaskResponse.cs`
- [X] T022 [US1] Emit mask-resolution telemetry during parse flow in `src/AddressFinder.FunctionApp/Functions/ParseAddressFunction.cs`

**Checkpoint**: US1 independently functional and testable.

---

## Phase 4: User Story 2 - Look Up Display Mask Directly By Country Code (Priority: P2)

**Goal**: Provide standalone mask lookup endpoint by ISO country code.

**Independent Test**: Call `GET /api/address/mask/{countryCode}` for valid, fallback, and invalid codes and verify deterministic response behavior.

### Tests for User Story 2 (Fail First)

- [X] T023 [P] [US2] Add contract test for mask lookup endpoint in `tests/AddressFinder.ContractTests/GetAddressMaskContractTests.cs`
- [X] T024 [P] [US2] Add integration tests for exact/fallback/invalid country lookup in `tests/AddressFinder.IntegrationTests/GetAddressMaskIntegrationTests.cs`
- [X] T025 [P] [US2] Add unit tests for ISO2/ISO3 resolution and normalization in `tests/AddressFinder.UnitTests/CountryCodeNormalizationTests.cs`

### Implementation for User Story 2

- [X] T026 [US2] Implement standalone lookup function in `src/AddressFinder.FunctionApp/Functions/GetAddressMaskFunction.cs`
- [X] T027 [US2] Implement lookup response contract in `src/AddressFinder.FunctionApp/Contracts/MaskLookupResponse.cs`
- [X] T028 [US2] Implement country code validation and 400 error path with request_id in `src/AddressFinder.FunctionApp/Functions/GetAddressMaskFunction.cs`
- [X] T029 [US2] Add OpenAPI annotations for lookup endpoint in `src/AddressFinder.FunctionApp/Functions/GetAddressMaskFunction.cs`

**Checkpoint**: US2 independently functional and testable.

---

## Phase 5: User Story 3 - Expose Mask Metadata For Caller Rendering Logic (Priority: P3)

**Goal**: Ensure metadata completeness (`mask_source`, `mask_version`, ordered dynamic `Line1..LineN`, and `max_line`) across both endpoints.

**Independent Test**: Validate both endpoint payloads always include required metadata and preserve line ordering.

### Tests for User Story 3 (Fail First)

- [X] T030 [P] [US3] Add contract tests for metadata presence on both endpoints in `tests/AddressFinder.ContractTests/MaskMetadataContractTests.cs`
- [X] T031 [P] [US3] Add integration tests verifying dynamic line ordering (`Line1..LineN`) and `max_line` correctness in `tests/AddressFinder.IntegrationTests/MaskMetadataIntegrationTests.cs`
- [X] T032 [P] [US3] Add unit tests for catalog_version and source propagation in `tests/AddressFinder.UnitTests/MaskCatalogMetadataTests.cs`

### Implementation for User Story 3

- [X] T033 [US3] Implement metadata propagation from catalog to response DTOs in `src/AddressFinder.FunctionApp/Domain/Services/MaskResolutionService.cs`
- [X] T034 [US3] Add dynamic line serialization safeguards and max_line derivation in `src/AddressFinder.FunctionApp/Domain/Models/CountryMaskDefinition.cs`
- [X] T035 [US3] Add metadata logging fields and troubleshooting tags in `src/AddressFinder.FunctionApp/Infrastructure/Telemetry/MaskResolutionTelemetry.cs`

**Checkpoint**: US3 independently functional and testable.

---

## Phase 6: User Story 4 - Handle Missing or Unsupported Country Masks Gracefully (Priority: P4)

**Goal**: Implement deterministic fallback behavior with preserved country metadata.

**Independent Test**: For valid-but-missing country mask, verify `200 OK`, fallback line layout, and original country metadata retention.

### Tests for User Story 4 (Fail First)

- [X] T036 [P] [US4] Add contract tests for fallback response shape and status in `tests/AddressFinder.ContractTests/FallbackMaskContractTests.cs`
- [X] T037 [P] [US4] Add integration tests for fallback metadata preservation in `tests/AddressFinder.IntegrationTests/FallbackMaskIntegrationTests.cs`
- [X] T038 [P] [US4] Add unit tests for fallback layout substitution logic in `tests/AddressFinder.UnitTests/FallbackResolutionTests.cs`

### Implementation for User Story 4

- [X] T039 [US4] Implement deterministic fallback layout selection in `src/AddressFinder.FunctionApp/Domain/Services/MaskResolutionService.cs`
- [X] T040 [US4] Preserve requested/parsed `Country`, `Iso3char`, and `Iso2char` during fallback in `src/AddressFinder.FunctionApp/Domain/Services/MaskResolutionService.cs`
- [X] T041 [US4] Ensure lookup endpoint returns `200 OK` for valid country without catalog entry in `src/AddressFinder.FunctionApp/Functions/GetAddressMaskFunction.cs`
- [X] T042 [US4] Add fallback warning details in response model in `src/AddressFinder.FunctionApp/Domain/Models/MaskResolutionWarning.cs`

**Checkpoint**: US4 independently functional and testable.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final hardening across stories.

- [X] T043 [P] Add end-to-end observability verification tests for fallback-rate and latency metrics in `tests/AddressFinder.IntegrationTests/ObservabilityIntegrationTests.cs`
- [X] T044 [P] Add redaction assertions for logs and error payloads in `tests/AddressFinder.UnitTests/PiiRedactionTests.cs`
- [X] T045 [P] Add unit tests for executable-syntax rejection in masks (FR-009) in `tests/AddressFinder.UnitTests/MaskContentSafetyValidatorTests.cs`
- [X] T046 Establish baseline and post-change measurement workflow for SC-004 in `tests/AddressFinder.IntegrationTests/RenderingFailureMetricsIntegrationTests.cs`
- [X] T047 Update function documentation and endpoint usage examples in `src/AddressFinder.FunctionApp/README.md`
- [X] T048 Run quick regression across parse + lookup flows and record results in `specs/002-country-display-mask/quick-validation.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 (Setup): starts immediately.
- Phase 2 (Foundational): depends on Phase 1 and blocks all user stories.
- Phase 3 (US1): depends on Phase 2.
- Phase 4 (US2): depends on Phase 2.
- Phase 5 (US3): depends on Phase 2 and benefits from US1/US2 contracts.
- Phase 6 (US4): depends on Phase 2 and mask resolution core from US1/US2.
- Phase 7 (Polish): depends on selected user stories being complete.

### User Story Completion Order

1. US1 (P1) MVP
2. US2 (P2)
3. US3 (P3)
4. US4 (P4)

### Parallel Opportunities

- Setup: T003, T004, T005, T006 can run in parallel.
- Foundational: T010-T014 can run in parallel after T007-T009 baseline.
- US1: T015-T018 parallel tests; T019-T022 sequential integration.
- US2: T023-T025 parallel tests; T026-T029 sequential integration.
- US3: T030-T032 parallel tests; T033-T035 sequential integration.
- US4: T036-T038 parallel tests; T039-T042 sequential integration.
- Polish: T043-T046 in parallel.

## Parallel Execution Examples

### User Story 1

- Run in parallel: T015, T016, T017, T018
- Then run sequentially: T019 -> T020 -> T021 -> T022

### User Story 2

- Run in parallel: T023, T024, T025
- Then run sequentially: T026 -> T027 -> T028 -> T029

### User Story 3

- Run in parallel: T030, T031, T032
- Then run sequentially: T033 -> T034 -> T035

### User Story 4

- Run in parallel: T036, T037, T038
- Then run sequentially: T039 -> T040 -> T041 -> T042

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Deliver Phase 3 (US1) as MVP.
3. Validate US1 independently before expanding scope.

### Incremental Delivery

1. Add US2 standalone lookup endpoint.
2. Add US3 metadata completeness.
3. Add US4 deterministic fallback hardening.

### Validation Gates

1. For each user story: tests fail first, then pass after implementation.
2. Verify no raw PII appears in logs/errors before story sign-off.
3. Verify OpenAPI contracts remain consistent with implemented responses.
