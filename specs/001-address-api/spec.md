# Feature Specification: Address Parsing API

**Feature Branch**: `001-address-api`  
**Created**: 2026-03-23  
**Status**: Draft  
**Input**: User description: "API that will take an address as a string, and return a structured json object based on the ISO 20022 AddressComposer standard"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Parse Unstructured Address into Structured JSON (Priority: P1)

A user provides a single-line or multi-line address string (e.g., "123 Main St, Springfield, IL 62701 USA") and the API parses it into a standardized, structured JSON object with discrete components (street, city, state, postal code, country).

**Why this priority**: This is the core MVP functionality. Parsing any address string into structured data is the fundamental value proposition and enables all downstream uses (validation, routing, matching).

**Independent Test**: Can be fully tested by sending sample address strings and validating that returned JSON contains expected fields (street_address, city, state_code, postal_code, country_code) with correct values.

**Acceptance Scenarios**:

1. **Given** a well-formed US address string "123 Main Street, Springfield, IL 62701 USA", **When** parsed, **Then** return JSON with street_address="123 Main Street", city="Springfield", state_code="IL", postal_code="62701", country_code="US"
2. **Given** a UK address string "42 Downing Street, London, SW1A 2AA, United Kingdom", **When** parsed, **Then** return JSON with correctly mapped UK fields (including postcode format)
3. **Given** a minimal address "New York, NY", **When** parsed, **Then** return JSON with non-empty city and state_code, optional fields null where not provided
4. **Given** an address with special characters "Café Saint-Pierre, 75001 Paris, France", **When** parsed, **Then** preserve special characters and return valid UTF-8 JSON

---

### User Story 2 - Address Validation and Standardization (Priority: P2)

The API validates parsed address components against known postal standards and return standardized formats (e.g., expand abbreviations "St" → "Street", normalize state codes to standard format).

**Why this priority**: P2 improves data quality and consistency. Once addresses are structured, validating them against postal standards prevents downstream errors in address matching or mailability checks.

**Independent Test**: Can be tested independently by sending addresses with non-standard formats (abbreviations, mixed case, etc.) and validating that returned JSON has standardized values without breaking P1 functionality.

**Acceptance Scenarios**:

1. **Given** "123 Main St.", **When** parsed, **Then** standardized_street_address="123 Main Street"
2. **Given** "springfield, il", **When** parsed, **Then** city="Springfield", state_code="IL" (normalized case)
3. **Given** a postal code "02134-1234", **When** parsed, **Then** postal_code="02134", postal_code_extension="1234" (if US format detected)
4. **Given** a country name "Great Britain", **When** parsed, **Then** country_code="GB" and country_name="United Kingdom" (normalized)

---

### User Story 3 - Timestamps and Audit Trail (Priority: P3)

The API includes timestamp information (when the address was parsed, potentially when it was last validated against reference data if available) in ISO 8601 format.

**Why this priority**: P3 enables audit trails and helps identify stale data. Essential for compliance but not blocking the core parsing feature.

**Independent Test**: Can be tested by checking that parsed address JSON includes valid RFC 3339 timestamps (parsed_at, validated_at where applicable).

**Acceptance Scenarios**:

1. **Given** any valid address, **When** parsed, **Then** response includes "parsed_at" with RFC 3339 timestamp (e.g., "2026-03-23T15:30:00Z")
2. **Given** an address successfully validated, **When** returned, **Then** response includes "validated_at" timestamp or null if validation skipped
3. **Given** timestamps in response, **When** serialized to JSON, **Then** all timestamps are RFC 3339 compliant and timezone-aware

---

### Edge Cases

- What happens when the input is an empty string or whitespace only? → Return 400 Bad Request with error message "Address string cannot be empty"
- How does system handle extremely long address strings (>500 characters)? → Accept up to reasonable limit (e.g., 1000 chars), gracefully reject oversized input with 400 Bad Request
- What if address contains only country information (no city/street)? → Return JSON with only country_code/country_name populated, other fields null
- How does system handle mixed-language addresses (e.g., Arabic script, Cyrillic)? → Accept and preserve UTF-8 encoded strings; return JSON with characters intact
- What happens if input contains malicious content (SQL injection patterns, script tags)? → Sanitize and escape in JSON output; do not execute or evaluate
- How does system handle addresses from countries with non-standard postal code formats? → Parse available fields; return postal_code field with whatever format is present; include postal_format hint if identifiable

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: API MUST accept address input as a single UTF-8 encoded string via HTTP POST request to /api/address/parse endpoint
- **FR-002**: API MUST return parsed address as JSON object with ISO 20022 AddressComposer fields: street_name, building_number, building_name, floor, room, po_box, postal_code, town_name, country_subdivision, country_code, country_name, and timestamps (additional optional fields per ISO 20022 as applicable)
- **FR-003**: API MUST handle addresses from multiple countries (minimum: US, UK, Canada, Germany, France, Japan)
- **FR-004**: API MUST standardize common abbreviations (e.g., "St" → "Street", "Rd" → "Road", "Ave" → "Avenue")
- **FR-005**: API MUST normalize state/province/region codes to standard format (e.g., ISO 3166-2 for US states)
- **FR-006**: API MUST return RFC 3339 formatted timestamps for all timestamp fields
- **FR-007**: API MUST include a "confidence_score" (0-100) indicating parser confidence in the result
- **FR-008**: API MUST handle addresses with missing components gracefully (return null for unavailable fields)

*Clarifications marked below:*

- **FR-009**: API MUST extract PO Box numbers to a separate "po_box" field in the response JSON per ISO 20022 AddressComposer; street_address field MUST NOT contain PO Box information
- **FR-010**: API MUST accept address strings up to 1000 characters in length; reject requests with input exceeding 1000 characters with 400 Bad Request error

### Operational & Privacy Requirements *(mandatory)*

- **OPR-001**: Specification MUST identify sensitive data involved in this feature:
  - Address data is personal information (PII) - street addresses can identify individuals
  - Do NOT log full address strings in application logs
  - Do NOT write raw address data to temporary files without encryption
  - Redact address information from error messages returned to clients
  - Parsed addresses WILL be stored in encrypted database with 90-day retention policy (automatic purge after 90 days of inactivity)
  - Cache key: SHA256 hash of normalized input (used for deduplication without storing raw addresses in cache index)
  - Encryption: All stored addresses encrypted at-rest using AES-256; encryption keys managed via secure key management service

- **OPR-002**: Data minimization and retention behavior:
  - STORE: Persist parsed addresses in encrypted database to support caching of repeated requests
  - CACHE-KEY: Use SHA256(normalized_input) for deduplication; do not index by raw address string
  - EXCLUDE: Do NOT capture user IP, device fingerprints, or metadata beyond UTC timestamp and request_id
  - RETENTION: Purge records automatically after 90 days of inactivity
  - Rationale: Address caching improves performance for repeated addresses while preserving privacy controls

- **OPR-003**: Observable signals needed for operations:
  - Structured logs (JSON) with: timestamp, request_id, input_hash (SHA256 of input for correlation without exposing PII), output_fields, confidence_score, parsing_duration_ms, error_type (if failed)
  - Metrics: requests_per_second, average_parse_latency_ms, parse_success_rate, confidence_score_distribution
  - Alerts: if success_rate drops below 95%, or average latency exceeds 500ms
  - Troubleshooting signals: Include request_id in error responses for log correlation

- **OPR-004**: Test expectations for each user story:
  - US1 (P1): Unit test: 50+ address samples from diverse sources parse without error; Integration test: end-to-end request/response validation
  - US2 (P2): Unit test: abbreviation normalization; Case normalization; Contact validation test with reference postal data (if available)
  - US3 (P3): Unit test: timestamp generation and RFC 3339 compliance; Integration test: verify timestamp in response is within 5 seconds of current time

### Key Entities

- **Address**: Composite entity representing a physical location address (ISO 20022 AddressComposer compliant)
  - Attributes (without implementation details, derived from ISO 20022 standard):
    - department: string or null (business unit or specific department)
    - street_name: string (name of the street)
    - building_number: string or null (number of the building)
    - building_name: string or null (name of the building)
    - floor: string or null (building floor identifier)
    - room: string or null (room number or name)
    - po_box: string or null (Post Box Number per ISO 20022)
    - postal_code: string (country-specific postal code format)
    - town_name: string (municipality or city name)
    - town_location_name: string or null (specific location within town, e.g., district)
    - district_name: string or null (larger district within country)
    - country_subdivision: string (ISO 3166-2 code for state/province/region)
    - country_code: string (ISO 3166-1 alpha-2 code, always 2 chars)
    - country_name: string (full English name of country)
    - address_line: string or null (optional additional address information)
    - confidence_score: integer (0-100, parser confidence in result)
    - parsed_at: RFC 3339 timestamp (when address was parsed)
    - cache_hit: boolean (true if result returned from cache, false if freshly parsed)
    - cached_at: RFC 3339 timestamp or null (when address was first cached, null if not cached)
    - validated_at: RFC 3339 timestamp or null (when address was validated, if applicable)

- **ParseError**: Error response entity
  - Attributes:
    - error_code: string (e.g., "INVALID_INPUT", "UNSUPPORTED_COUNTRY")
    - error_message: string (user-friendly message, redacted of PII)
    - request_id: string (for troubleshooting correlation)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: API returns parsed address JSON within 500ms for 95th percentile of requests (measured across diverse address types)
- **SC-002**: System achieves ≥95% parse success rate across training dataset of 1,000+ diverse global addresses (measured by valid JSON response with non-empty required fields)
- **SC-003**: Parsed output matches human-verified ground truth for 90% of test cases (measured against manually validated address samples)
- **SC-004**: API handles 1,000 concurrent requests without degradation (measured by maintaining <500ms p95 latency under load)
- **SC-008**: Cache hit rate ≥30% for typical user workloads (measured by tracking cache_hit=true responses over 1-week period)
- **SC-009**: Cached address returned within 50ms (50th percentile) and 100ms (95th percentile) including database lookup and decryption
- **SC-005**: Zero instances of unredacted PII in production logs or error responses (measured through automated log scanning and manual audit)
- **SC-006**: Timestamp accuracy: all "parsed_at" timestamps are within ±1 second of actual parsing time (measured through clock correlation)
- **SC-007**: Average confidence_score ≥75 for well-formed addresses; <50 for ambiguous/partial addresses (user-facing metric for parsing quality)

## Assumptions *(optional)*

- **Address Schema**: The Address entity is designed to comply with ISO 20022 AddressComposer standard, which defines a structured hierarchy of address components. Key ISO 20022 elements included: Department, Street Name, Building Number/Name, Floor, Room, Post Box Number, Postal Code, Town Name (city), Town Location, District, Country Subdivision (state/province), Country Code, and optional Address Line.
- **PO Box Handling**: ISO 20022 standard defines Post Box Number as a separate, first-class element. This decision (Q1: Option A) ensures standards compliance and enables downstream routing/filtering logic.
- **Field Optionality**: Not all ISO 20022 fields will be present for every address (e.g., building_name, floor, room, department are optional). The API returns null for unparseable or unavailable fields.
- **Country Support**: Minimum viable set includes US, UK, Canada, Germany, France, Japan; additional countries can be supported through postal reference data expansion.
- **Caching & Privacy**: Per OPR-001, cached addresses are encrypted at-rest using AES-256 and purged after 90 days of inactivity. Cache keys use SHA256 hashing of normalized input to avoid exposing PII in cache indexes.
