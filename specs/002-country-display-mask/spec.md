# Feature Specification: Country Display Masks For Parsed Addresses

**Feature Branch**: `002-country-display-mask`  
**Created**: 2026-03-23  
**Status**: Draft  
**Input**: User description: "The response should include a mask from Loqate international address formats so the caller knows how to format any display for the specific country"

## Clarifications

### Session 2026-03-23

- Q: Should this feature extend the existing parse endpoint or be a separate endpoint? → A: Both — extend `POST /api/address/parse` to include mask fields in the response AND expose a new standalone `GET /api/address/mask/{countryCode}` endpoint for direct mask lookup by country code.
- Q: How is the Loqate country mask catalog loaded into the running service? → A: Embedded JSON config file — deployed with the function app and loaded at startup; no runtime dependency on external data store.
- Q: What shape should the returned mask use? → A: A structured `mask` object containing country metadata (`Country`, `Iso3char`, `Iso2char`) and ordered dynamic `Line1`..`LineN` arrays of component names (for example `Line1: ["Organization"]`, `Line8: ["PostalCode", "Locality"]`).
- Q: For `GET /api/address/mask/{countryCode}`, what should happen when the country code is valid but there is no configured country-specific mask? → A: Return `200 OK` with the deterministic fallback mask and `mask_resolution_status = "fallback"`.
- Q: When a fallback mask is returned, what should the `mask` object's country metadata contain? → A: Keep `Country`, `Iso3char`, and `Iso2char` for the requested or parsed country; only the `Line1`..`LineN` layout falls back to the default definition.
- Q: Should mask output support lines beyond `Line8`? → A: Yes. Return dynamic `Line1`..`LineN` arrays and include `mask.max_line` metadata indicating the highest line index present.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Return Country Display Mask With Parse Result (Priority: P1)

As an API consumer, I receive a country-specific display mask with every successful parse response so I can render addresses in the expected local format.

**Why this priority**: This is the direct business outcome requested. Without the mask, callers cannot reliably format display output by country.

**Independent Test**: Can be fully tested by parsing addresses from supported countries and verifying response includes a non-empty display mask sourced from the Loqate reference set.

**Acceptance Scenarios**:

1. **Given** a valid US address input, **When** parsing succeeds via `POST /api/address/parse`, **Then** response includes country code and the US display mask for that country.
2. **Given** a valid GB address input, **When** parsing succeeds via `POST /api/address/parse`, **Then** response includes country code and the GB display mask for that country.
3. **Given** a valid address with PO Box content, **When** parsing succeeds, **Then** response includes PO Box data and a display mask that supports PO Box placement for that country.

---

### User Story 2 - Look Up Display Mask Directly By Country Code (Priority: P2)

As an API consumer, I can request the display mask for a specific country without performing a full address parse, so I can pre-load rendering templates for known countries.

**Why this priority**: Enables callers to initialise rendering logic upfront (e.g., form rendering, print templates) without needing to parse an address first.

**Independent Test**: Can be fully tested by calling `GET /api/address/mask/{countryCode}` with a valid ISO 3166-1 alpha-2 code and verifying a mask definition is returned independently of the parse pipeline.

**Acceptance Scenarios**:

1. **Given** a valid country code `US`, **When** caller requests `GET /api/address/mask/US`, **Then** response contains `mask` (including `Country`, `Iso3char`, `Iso2char`, ordered `Line1`..`LineN` arrays, and `max_line`), `mask_source`, and `mask_version` for the US.
2. **Given** a valid country code with no configured mask, **When** caller requests the endpoint, **Then** API returns `200 OK` with the deterministic fallback mask and `mask_resolution_status = fallback`.
3. **Given** an invalid or malformed country code, **When** caller requests the endpoint, **Then** API returns a 400 error with `request_id` and no raw data in error payload.

---

### User Story 3 - Expose Mask Metadata For Caller Rendering Logic (Priority: P3)

As an API consumer, I receive enough metadata with the mask — from either endpoint — to apply rendering safely and understand which reference version produced it.

**Why this priority**: Provenance and placeholder details prevent inconsistent rendering and help debug mismatches across environments.

**Independent Test**: Can be tested by verifying both endpoints return `mask_source`, `mask_version`, and a `mask` object with ordered dynamic `Line1`..`LineN` arrays plus `max_line` in every successful response.

**Acceptance Scenarios**:

1. **Given** a successful parse response, **When** inspecting payload, **Then** it contains `mask`, `mask_source`, and `mask_version`.
2. **Given** a successful mask lookup response, **When** inspecting payload, **Then** it contains ordered `Line1`..`LineN` arrays for display composition and a `max_line` value.

---

### User Story 4 - Handle Missing or Unsupported Country Masks Gracefully (Priority: P4)

As an API consumer, I receive deterministic fallback behavior when no country mask exists so the application remains usable.

**Why this priority**: Fallback behavior prevents caller failures and avoids blocking user workflows for newly added or rare country patterns.

**Independent Test**: Can be tested by sending an address for a country with no configured mask and verifying fallback format and warning signals are returned.

**Acceptance Scenarios**:

1. **Given** a country with no configured mask, **When** parsing succeeds, **Then** response contains a deterministic fallback mask and a warning flag indicating default mask use.
2. **Given** a malformed country code, **When** parsing is attempted, **Then** API returns a validation error with request identifier and no raw address in error text.

---

### Edge Cases

- Address parses successfully but country cannot be confidently identified: return fallback mask and set confidence-based warning.
- Country is identified but mask entry is missing from reference data: return fallback mask and include `mask_resolution_status = "fallback"`.
- Fallback mask is used for a known country: preserve requested or parsed country metadata in `Country`, `Iso3char`, and `Iso2char`; only the dynamic line layout (`Line1`..`LineN`) comes from the fallback definition.
- Mask contains line components not present in parsed output fields: return empty values for missing components and include component-mismatch warning.
- Input exceeds 1000 characters: return validation error without returning mask data.
- Reference mask catalog update introduces changed component names: maintain backward compatibility in response via alias mapping for one version cycle.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: API MUST return a country-specific `mask` object in every successful `POST /api/address/parse` response.
- **FR-001b**: API MUST expose a standalone `GET /api/address/mask/{countryCode}` endpoint that returns the `mask` object definition for any given ISO 3166-1 alpha-2 country code without requiring a full address parse.
- **FR-002**: The `mask` object MUST be sourced from a versioned JSON catalog file embedded in the function app deployment, derived from the Loqate international address formats reference document.
- **FR-002b**: The embedded catalog file MUST include a `catalog_version` field so callers and operators can identify which Loqate reference edition is active.
- **FR-002c**: The service MUST load the catalog at startup and cache it in-process for the lifetime of the function host; no per-request file I/O is permitted.
- **FR-003**: API MUST return `mask_source` and `mask_version` fields indicating the authoritative source and version used to resolve the mask.
- **FR-004**: API MUST return `mask.Country`, `mask.Iso3char`, `mask.Iso2char`, ordered `mask.Line1` through `mask.LineN` arrays matching the configured country definition, and `mask.max_line` indicating the highest available line index.
- **FR-004b**: Each `mask.LineN` entry MUST contain an ordered array of component names to be rendered on that display line, not executable template syntax.
- **FR-005**: API MUST return `mask_resolution_status` with value `exact` when a country-specific mask is found and `fallback` when default mask is used.
- **FR-006**: API MUST provide a deterministic fallback mask for countries without a configured Loqate mask entry.
- **FR-006b**: `GET /api/address/mask/{countryCode}` MUST return `200 OK` with the fallback mask when the country code is valid but no country-specific catalog entry exists.
- **FR-006c**: When fallback mask resolution occurs for a known country, API MUST preserve `mask.Country`, `mask.Iso3char`, and `mask.Iso2char` for that country and replace only `mask.Line1` through `mask.LineN` with the fallback layout.
- **FR-007**: API MUST preserve existing parsed address fields and ISO 20022-aligned elements while adding mask fields.
- **FR-008**: API MUST include PO Box handling in mask resolution so countries with explicit PO Box positioning render correctly.
- **FR-009**: API MUST ensure response never includes executable template syntax in `mask`; only declarative component-name arrays are allowed.
- **FR-010**: API MUST reject invalid input (empty, whitespace-only, or >1000 chars) with structured error response and request identifier.

### Operational & Privacy Requirements *(mandatory)*

- **OPR-001**: Sensitive data includes raw address input and parsed address components; logs and diagnostics MUST redact or hash address values.
- **OPR-002**: Data minimization MUST limit stored mask data to country metadata, ordered line-component arrays, mask identifier, and version metadata; no extra personal data collected for mask resolution.
- **OPR-003**: Operational signals MUST include `mask_resolution_status`, `mask_version`, parse latency, and fallback-rate metrics for production monitoring.
- **OPR-004**: Test expectations MUST include fail-first tests for exact mask lookup, fallback behavior, metadata presence, and input validation errors.

### Key Entities *(include if feature involves data)*

- **CountryMaskDefinition**: Country-level display template metadata derived from Loqate reference.
  - Attributes:
    - Country: string (display country name)
    - Iso3char: string (ISO 3166-1 alpha-3)
    - Iso2char: string (ISO 3166-1 alpha-2)
    - Line1..LineN: ordered lists of component names (dynamic line count per country)
    - max_line: integer (highest line index available for this country)
    - supports_po_box: boolean
    - mask_version: string
    - mask_source: string

- **ParsedAddressWithMask**: Parsed address payload augmented with display mask information.
  - Attributes:
    - parsed_address: object (ISO 20022-aligned parsed address fields)
    - mask: object containing `Country`, `Iso3char`, `Iso2char`, dynamic `Line1`..`LineN`, and `max_line`; on fallback, country metadata remains specific to the requested or parsed country
    - mask_resolution_status: enum (`exact`, `fallback`)
    - mask_version: string
    - mask_source: string
    - request_id: string

- **MaskResolutionWarning**: Non-fatal warning details for fallback and component mismatch cases.
  - Attributes:
    - warning_code: string
    - warning_message: string
    - mask_resolution_status: enum (`fallback`, `partial`)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of successful parse responses include non-empty `mask`, `mask_source`, and `mask_version`.
- **SC-002**: At least 95% of requests from supported countries resolve to `mask_resolution_status = exact`.
- **SC-003**: For unsupported countries, 100% of successful responses return deterministic fallback mask with explicit `fallback` status.
- **SC-003b**: For valid country codes without country-specific catalog entries, 100% of standalone mask lookup responses return `200 OK` with deterministic fallback mask and `mask_resolution_status = fallback`.
- **SC-004**: Caller-facing address rendering failures attributable to missing format guidance are reduced by at least 80% after rollout.
- **SC-005**: p95 additional latency introduced by mask resolution is <= 50ms per request.
- **SC-006**: 0 instances of raw address strings appearing in mask-resolution logs or error payloads.

## Assumptions *(optional)*

- Loqate international address format reference is treated as the authoritative catalog for country display masks in this feature.
- Catalog is delivered as a versioned JSON file embedded in the function app deployment; updates are applied by redeploying with the new file.
- Catalog file is loaded once at function host startup and held in-process memory; no runtime calls to external services are needed for mask resolution.
- Returned mask uses a structured object with country metadata, dynamic ordered `Line1`..`LineN` arrays of component names, and `max_line` metadata rather than a single tokenized format string.
- When fallback is used, country identity remains specific to the requested or parsed country; only the line layout falls back.
- Fallback mask format is stable across environments to guarantee deterministic caller behavior.
