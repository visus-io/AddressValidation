# Translation Style Guide

This style guide is intended for translators working on the AddressValidation project via Crowdin.
Please read it carefully before submitting any translations.

## Introduction

AddressValidation is a .NET library that validates physical mailing addresses against external provider
APIs (FedEx, Google, Pitney Bowes, UPS). The strings in this project are **validation error messages**
surfaced to developers at runtime when an address validation request fails or produces a warning.

The audience is **software developers**, not end users. Messages are typically logged or returned in API
responses, so they must be precise, unambiguous, and professional.

## Tone & Voice

- Use a **professional and technical** tone.
- Keep sentences **short and direct**. Avoid verbose constructions.
- Do **not** use contractions (e.g. use "cannot" not "can't", "does not" not "doesn't").
- Do **not** add explanatory or apologetic language that is absent from the source string.
- Preserve the level of formality of the source. These are error messages, not UI copy.

## Placeholders

Several strings contain positional .NET format arguments: `{0}`, `{1}`, `{2}`, `{3}`.

**Rules:**

- Every placeholder present in the source string **must** appear in the translation, unchanged.
- Do **not** alter the braces, the index number, or any surrounding whitespace that is part of the
  placeholder token (e.g. `{0}` must remain `{0}`, not `{ 0 }` or `(0)`).
- You **may** reorder placeholders to match the natural word order of the target language
  (e.g. `{1}` may appear before `{0}` if the target language requires it).
- Never omit a placeholder, even if the translated sentence feels complete without it.

### Placeholder reference

| Placeholder | Typical content |
|---|---|
| `{0}` | Field or property name (e.g. `PostalCode`, `CountryCode`) |
| `{1}` | The offending value submitted in the request |
| `{2}` | Provider name (e.g. `FedEx`, `UPS`) — see [Do Not Translate](#do-not-translate) |
| `{3}` | Operation mode (e.g. `PRODUCTION`, `DEVELOPMENT`) |

In `Validation_Verification_RowValueCouldNotBeVerified`, the placeholders carry different meanings:

| Placeholder | Content |
|---|---|
| `{0}` | Row index (integer) |
| `{1}` | Field or property name |
| `{2}` | Nested error message |

## Do Not Translate

The following terms must **always** be left in English, exactly as written:

| Term | Reason |
|---|---|
| `null` | .NET keyword; altering it would confuse developers |
| `FedEx` | Registered trademark and provider name |
| `Google` | Provider name |
| `Pitney Bowes` | Provider name |
| `UPS` | Provider name |
| `USPS` | Provider name |
| `Address Validation API` | Product name used verbatim in provider documentation |
| `{0}`, `{1}`, `{2}`, `{3}` | Format placeholders — see [Placeholders](#placeholders) |

## Punctuation

- Source strings end with a **period**. Preserve this in all translations.
- The string `Validation_Verification_RowValueCouldNotBeVerified` uses the structural pattern
  `[Row {0}] {1}: {2}`. Keep the square brackets `[` `]` and the colon-space `: ` separator exactly
  as they appear. Only the surrounding prose (if any) should be translated — in this case, the word
  `Row` may be translated.

## Capitalization

- Follow the capitalization conventions of the **target language**.
- Do not reproduce English title casing in languages that do not use it.
- Proper nouns and trademarks (FedEx, Google, etc.) retain their original casing regardless of
  target-language rules.

## String Reference

The table below lists every translatable string in this project. Use it to understand the full context
before translating.

### `src/Visus.AddressValidation/Resources/Resources.resx`

| Key | English source | Notes |
|---|---|---|
| `Validation_Field_CannotBeNullOrEmpty` | `{0}: Value cannot be null or empty.` | `{0}` = field name. "null" must not be translated. |
| `Validation_Field_MustBeBetween` | `{0}: Value must be between {1} and {2}.` | `{0}` = field name; `{1}` and `{2}` = numeric bounds. |
| `Validation_Address_LinesCannotExceedThree` | `Cannot contain more than 3 entries.` | Refers to the address lines collection. No placeholders. |
| `Validation_Address_CountryNotSupported` | `{0}: {1} is currently not supported for address validation.` | `{0}` = field name (typically `CountryCode`); `{1}` = the country code value. |
| `Validation_Provider_CountryNotSupported` | `{0}: {1} is currently not supported by the {2} Address Validation API.` | `{0}` = field name; `{1}` = country code value; `{2}` = provider name (do not translate). |
| `Validation_Provider_OnlyValueSupportedInMode` | `{0}: Only the value {1} is supported by the {2} Address Validation API while in {3} mode.` | `{0}` = field name; `{1}` = the single allowed value; `{2}` = provider name; `{3}` = mode name. All four placeholders are required. |
| `Validation_Provider_OnlyValuesSupportedInMode` | `{0}: Only the values {1} are supported by the {2} Address Validation API while in {3} mode.` | Same as above but `{1}` is a formatted list of multiple values (plural form). All four placeholders are required. |
| `Validation_Verification_ValueCouldNotBeVerified` | `Value could not be verified.` | Generic fallback message. No placeholders. |
| `Validation_Verification_RowValueCouldNotBeVerified` | `[Row {0}] {1}: {2}` | Structural message. `[Row {0}]` is a row index prefix; `{1}` = field name; `{2}` = nested error message. Keep `[`, `]`, and `: ` intact. "Row" may be translated. |

### `src/Visus.AddressValidation.Integration.FedEx/Resources/Resources.resx`

| Key | English source | Notes |
|---|---|---|
| `Validation_FedEx_InvalidSuiteNumber` | `Invalid suite number was provided in the request.` | FedEx-specific. No placeholders. |
| `Validation_FedEx_SuiteNumberNotProvided` | `Suite number was not provided in the request.` | FedEx-specific. No placeholders. |
