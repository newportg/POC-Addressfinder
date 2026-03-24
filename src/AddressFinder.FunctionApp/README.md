# AddressFinder Function App

## Endpoints

- `POST /api/address/parse`
  - Request: `{ "address_input": "..." }`
  - Response: includes `parsed_address`, `mask`, `mask_resolution_status`, `mask_version`, `mask_source`, `request_id`

- `GET /api/address/mask/{countryCode}`
  - Response: includes `mask` and metadata fields for caller rendering
  - Valid but unsupported countries return `200 OK` with `mask_resolution_status = "fallback"`

## Mask Payload Shape

`mask` contains:

- `Country`
- `Iso3char`
- `Iso2char`
- dynamic `Line1..LineN` arrays
- `max_line`

## Catalog

Mask catalog file: `Infrastructure/Catalog/country-mask-catalog.json`

- loaded at startup
- cached in process
- includes `catalog_version` and `mask_source`
