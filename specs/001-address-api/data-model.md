# Data Model: Address Parsing API

## Entity: ParseAddressRequest
- Purpose: Input envelope for parse endpoint.
- Fields:
  - address_input: string, required, max length 1000, UTF-8.
  - request_id: string, optional (server-generated when missing).
- Validation rules:
  - Reject empty/whitespace input with `INVALID_INPUT`.
  - Reject input length > 1000 with `INPUT_TOO_LONG`.

## Entity: ParsedAddress
- Purpose: ISO 20022-aligned structured output.
- Fields:
  - department: string|null
  - sub_department: string|null
  - street_name: string|null
  - building_number: string|null
  - building_name: string|null
  - floor: string|null
  - post_box_number: string|null
  - room: string|null
  - postal_code: string|null
  - town_name: string|null
  - town_location_name: string|null
  - district_name: string|null
  - country_subdivision: string|null
  - country: string|null (ISO 3166-1 alpha-2)
  - address_line: string|null
  - confidence_score: int (0..100)
  - parsed_at: string (RFC 3339 UTC)
  - validated_at: string|null (RFC 3339 UTC)
  - cache_hit: bool
  - cached_at: string|null (RFC 3339 UTC)
- Validation rules:
  - `country` must be two uppercase letters when present.
  - `confidence_score` must remain in [0, 100].
  - Timestamp fields must be RFC 3339.

## Entity: ParseError
- Purpose: Standard error contract.
- Fields:
  - error_code: string (e.g., INVALID_INPUT, INPUT_TOO_LONG, UNSUPPORTED_COUNTRY)
  - error_message: string (redacted, no raw PII)
  - request_id: string

## Entity: CachedParseRecord
- Purpose: Persistent encrypted response store for repeated requests.
- Fields:
  - cache_key: string (SHA256 of normalized input)
  - encrypted_payload: string (AES-256 encrypted JSON)
  - created_utc: string (RFC 3339 UTC)
  - last_accessed_utc: string (RFC 3339 UTC)
  - expires_utc: string (RFC 3339 UTC, 90-day TTL)
- Validation rules:
  - Never persist raw input as index key.
  - Encrypted payload only; no plaintext address columns.

## Relationships
- ParseAddressRequest 1..1 -> ParsedAddress on success.
- ParseAddressRequest 1..1 -> ParseError on failure.
- ParsedAddress 0..1 <-> CachedParseRecord via `cache_key`.

## State Transitions
- Request lifecycle:
  - Received -> Validated -> CacheLookup
  - CacheLookup -> CacheHit -> Returned
  - CacheLookup -> CacheMiss -> Parsed -> Normalized -> Stored -> Returned
  - Any state -> Failed -> ParseErrorReturned
