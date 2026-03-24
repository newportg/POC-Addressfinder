# Specification Quality Checklist: Address Parsing API

**Purpose**: Validate specification completeness and quality before proceeding to planning  
**Created**: 2026-03-23  
**Feature**: [001-address-api/spec.md](spec.md)

## Content Quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

## Requirement Completeness

- [x] No [NEEDS CLARIFICATION] markers remain (ALL RESOLVED)
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Success criteria are technology-agnostic (no implementation details)
- [x] All acceptance scenarios are defined
- [x] Edge cases are identified
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

## Feature Readiness

- [x] All functional requirements have clear acceptance criteria
- [x] User scenarios cover primary flows
- [x] Feature meets measurable outcomes defined in Success Criteria
- [x] No implementation details leak into specification

## All Clarifications Resolved ✅

All three critical clarifications have been addressed:

### ✅ Question 1: PO Box Handling (RESOLVED)

**Location**: FR-009  
**Choice**: A - **Extract to separate field (ISO 20022 Compliant)**  
**Rationale**: ISO 20022 AddressComposer standard defines Post Box Number as a distinct field (not part of street address). This ensures:
- Standards alignment (ISO 20022 compliance)
- Structured data model supporting business logic and routing
- Clean field separation enabling filtering by address type
- Compliance with international postal standards
  
**Implementation**: 
- New "po_box" field in response JSON (null if not present)
- PO Box numbers extracted and returned separately
- street_name field never contains PO Box information
- Entire Address entity aligned to ISO 20022 AddressComposer schema
- Example: "PO Box 123, Springfield, IL 62701 USA" → `po_box: "123"`, `street_name: null`, `town_name: "Springfield"`, `country_subdivision: "IL"`

---

### ✅ Question 2: Input Size Limit (RESOLVED)

**Location**: FR-010  
**Choice**: B - **1000 characters**  
**Rationale**: Balanced approach; allows multi-line, detailed addresses; typical API limit  
**Implementation**: API accepts addresses up to 1000 characters; rejects longer inputs with 400 Bad Request

---

### ✅ Question 3: Address Persistence (RESOLVED)

**Location**: OPR-001  
**Choice**: C - **Full address storage with caching**  
**User Specification**: "If the same address is asked for repeatedly then a stored response should be returned"

**Implementation details**:
- Store parsed addresses in encrypted database
- Cache key: SHA256(normalized_input) for deduplication
- Cache lookup ensures repeated requests return immediately (stored response)
- Retention policy: 90-day automatic purge after inactivity
- Encryption: AES-256 at-rest; keys managed via secure key management service
- Added fields to Address entity: `cache_hit` (boolean), `cached_at` (timestamp)
- Added success criteria: ≥30% cache hit rate; cached responses <100ms p95

---

## Notes

- **Remaining**: Only Q1 (PO Box handling) requires your response to fully complete the spec
- Spec structure and quality are complete and compliant with constitution principles
- Once Q1 is resolved, all [NEEDS CLARIFICATION] markers will be eliminated
- Ready to proceed to `/speckit.plan` phase after Q1 response
ext Steps

✅ **Specification Complete & ISO 20022 Aligned**  
All clarifications resolved. Spec is fully compliant with:
- ISO 20022 AddressComposer standard (Address schema derived from official ISO 20022 elements)
- Constitution principles: Spec-Driven ✅ | Privacy-First ✅ | Test-First ✅ | Observable ✅ | Simple ✅

**Address Schema Elements Incorporated**:
- Department, Sub Department (business units) - Optional fields supported
- Street Name, Building Number, Building Name (street/building details) - ISO 20022 mapped
- Floor, Room (precise location) - ISO 20022 mapped
- Post Box Number (extracted to separate field) - ISO 20022 mapped
- Postal Code - ISO 20022 mapped
- Town Name, Town Location, District (location hierarchy) - ISO 20022 mapped
- Country Subdivision (state/province) - ISO 20022 mapped
- Country (2-char ISO 3166 code) - ISO 20022 mapped
- Address Line (flexible additional info) - ISO 20022 mapped

**Next action**: Run `/speckit.plan` to generate implementation plan, data model, and design artifacts