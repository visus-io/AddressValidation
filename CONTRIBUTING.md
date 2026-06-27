# Contributing to AddressValidation

Thank you for your interest in contributing. This document explains the process for reporting issues, proposing changes, and submitting pull requests.

Please read this guide before opening an issue or submitting a pull request. For questions about the internal design, refer to [ARCHITECTURE.md](ARCHITECTURE.md).

## Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating you agree to abide by its terms.

## Reporting Security Vulnerabilities

**Do not open a public issue for security vulnerabilities.** Email [admin@projects.visus.io](mailto:admin@projects.visus.io) with a description of the vulnerability, steps to reproduce, and any relevant version information. You will receive a response within 72 hours.

## Reporting Bugs

Before opening a bug report, search the [existing issues](https://github.com/visus-io/AddressValidation/issues) to check whether the problem has already been reported.

When filing a new bug, include:

- A clear, concise description of the problem
- The target framework version and OS
- Minimal reproduction steps or a failing test case
- The actual behaviour and what you expected instead

## Requesting Features

Open a [feature request](https://github.com/visus-io/AddressValidation/issues/new) describing the use case before writing any code. This allows the maintainers to discuss feasibility and design before implementation work begins. Pull requests for non-trivial new features that arrive without a prior issue may be closed without review.

## Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — exact version pinned in [global.json](global.json)
- Any editor with C# support (Visual Studio, Rider, VS Code with C# Dev Kit)

### Building

```shell
dotnet build
```

### Running the Tests

```shell
dotnet test
```

Integration tests use [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) and never call live provider APIs. No credentials are required to run the test suite.

#### Updating Public API Snapshots

After changing any public API surface, regenerate the snapshot files before committing:

```shell
VERIFY_UPDATE=1 dotnet test
```

Commit the updated `*.verified.txt` files alongside your code changes.

## Submitting a Pull Request

1. [Fork](https://github.com/visus-io/AddressValidation/fork) the repository.
2. Create a branch from `main` with a short descriptive name (e.g., `feat/ups-suggestions`, `fix/postal-code-normalization`).
3. Make your changes following the code style rules below.
4. Add or update tests to cover your changes. All new public types and members must be tested.
5. Run the full test suite locally and confirm it passes.
6. Push your branch and open a pull request against `main`.

Pull requests should be focused — one logical change per PR. If your work spans multiple concerns, split them into separate PRs.

## Developer Certificate of Origin

This project requires a [Developer Certificate of Origin (DCO)](https://developercertificate.org/) sign-off on every commit. Add it by passing `-s` to `git commit`:

```shell
git commit -s -m "feat(ups): add IncludeSuggestions support"
```

This appends a `Signed-off-by: Your Name <your@email.com>` line to the commit message, certifying that you wrote the change or have the right to submit it under the project's license. Pull requests with unsigned commits will be blocked by the DCO check.

## Commit Messages

This project uses [Conventional Commits](https://www.conventionalcommits.org/). Every commit message must follow the format:

```
<type>(<scope>): <short summary>
```

Common types:

| Type | When to use |
|---|---|
| `feat` | A new feature or capability |
| `fix` | A bug fix |
| `docs` | Documentation changes only |
| `test` | Adding or updating tests |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `chore` | Maintenance tasks (dependency updates, build changes, etc.) |

The scope is optional but encouraged — use the affected package or area (e.g., `fedex`, `core`, `source-gen`).

Examples:

```
feat(ups): add IncludeSuggestions support
fix(google): correct StateOrProvince for city-state countries
docs: add CONTRIBUTING guide
chore(deps): update WireMock.Net to 2.11.0
```

## Code Style

All contributions must follow the project's code style rules, which are enforced by the build:

- No `var` — always use explicit types.
- Allman braces — opening `{` on its own line.
- Explicit accessibility modifiers on every member.
- `s_` prefix for `private static` fields.
- `ConfigureAwait` is required on every `await` expression — violations are build errors.
- All public members must carry XML doc comments (`<summary>` at minimum).
- All user-facing validation error messages must be defined in the project's `Resources/Resources.resx` file. Never hardcode message strings in C# source.

The full set of rules is defined in [.editorconfig](.editorconfig) and [AGENTS.md](AGENTS.md).

## Adding a New Provider Integration

New provider integrations must follow the canonical project layout described in [ARCHITECTURE.md](ARCHITECTURE.md) and satisfy the Port/Adapter and Template Method patterns used throughout the codebase. Open a feature request first so the design can be discussed before implementation.

## Licensing

By submitting a pull request you agree that your contribution will be licensed under the [MIT License](LICENSE) that covers this project.
