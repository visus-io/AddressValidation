# Security Policy

## Supported Versions

Only the latest published version of each package receives security fixes. Older minor or patch releases are not backported.

| Package | Supported |
|---|---|
| `Visus.AddressValidation` | Latest version only |
| `Visus.AddressValidation.Integration.FedEx` | Latest version only |
| `Visus.AddressValidation.Integration.Google` | Latest version only |
| `Visus.AddressValidation.Integration.PitneyBowes` | Latest version only |
| `Visus.AddressValidation.Integration.Ups` | Latest version only |
| `Visus.AddressValidation.SourceGeneration` | Latest version only |

## Reporting a Vulnerability

**Do not open a public GitHub issue for security vulnerabilities.**

Email [security@projects.visus.io](mailto:security@projects.visus.io) with the following information:

- The affected package(s) and version(s)
- A clear description of the vulnerability
- Steps to reproduce or a proof-of-concept
- An assessment of the potential impact

You will receive an initial response within **72 hours** confirming receipt and indicating whether the report is in scope.

## Coordinated Disclosure

Once a report is accepted, the maintainers will:

1. Confirm the scope and severity of the vulnerability.
2. Develop and test a fix on a private branch.
3. Release a patched version and request a CVE assignment where applicable.
4. Notify the reporter before the fix is made public.

We ask that you observe a **90-day embargo** from the date of your initial report to allow time for a coordinated release. If special circumstances require an earlier disclosure, please discuss this with the maintainers before publishing.

## Scope

The following are considered security vulnerabilities in this library:

- Credential or token leakage — for example, API keys or OAuth tokens appearing in log output, exception messages, or serialized responses.
- A transitive dependency with a published CVE that directly affects consumers of this library.
- Insecure defaults in the DI registration or HTTP client configuration (e.g., TLS downgrade, disabled certificate validation).

## Out of Scope

The following are **not** considered security vulnerabilities in this library:

- Bugs or unexpected behavior that have no security impact — please use the [bug report](https://github.com/visus-io/AddressValidation/issues/new/choose) template instead.
- Vulnerabilities in the upstream provider APIs (FedEx, Google, Pitney Bowes, UPS) — report those directly to the respective provider.
- Issues present only in test helper code that is never shipped to consumers.

## Preferred Languages

Please submit all reports in English.

---

*Last updated: 2026-07-07*
