<!-- architecture -->
@ARCHITECTURE.md
<!-- architecture -->

# Agent Instructions

## Language and Runtime

- Target framework is `net10.0`. The Roslyn source generator targets `netstandard2.0`.
- Language version is C# 14 (`LangVersion 14` in `Directory.Build.props`).
- All source packages carry `IsAotCompatible = true`. Never introduce reflection-based APIs (e.g., `Type`-accepting `JsonSerializer` overloads, `Activator.CreateInstance`, `MethodInfo.Invoke`).

## Code Style

- **No `var`** — always use explicit types.
- **Allman braces** — opening `{` on its own line.
- **Explicit accessibility modifiers** on every member.
- **`s_` prefix** for `private static` fields.
- **`ConfigureAwait` is required** on every `await` expression — violations are build errors.
- **`using` directives inside the namespace** — never before the `namespace` declaration (`.editorconfig` enforces this).
- **`using` sort order** — `System.*` namespaces first, then all other namespaces alphabetically; `using static` sorted among them by namespace. ReSharper *Full Cleanup* enforces this automatically.
- **Member ordering** — members are grouped by access level (public → protected → internal → private). Within each access group the order is: constants/static fields → instance fields → constructors → destructors → delegates → events → enums → interfaces → properties → indexers → methods → nested types (structs, classes, records). Within each category, sort by: static before instance, readonly before mutable, then alphabetically by name. ReSharper *Full Cleanup* enforces this automatically.
- All public members must carry XML doc comments (`<summary>` at minimum).
- Follow the rules in `.editorconfig` exactly. Do not override them with inline suppression unless there is a documented reason.

## Project Structure Rules

- New provider integrations go under `src/` and must mirror the canonical directory layout described in ARCHITECTURE.md (Abstractions, Adapters, Clients, Configuration, Constants.cs, Contracts, Extensions, Mappers, Models, Resources, Serialization, Services, Validation).
- New test projects go under `tests/` and must mirror their source counterpart.
- Never add a version attribute to a `<PackageReference>` — all versions are managed centrally in `Directory.Packages.props`.
- Never add a new NuGet package without also adding its version pin to `Directory.Packages.props`.

## Patterns to Follow

- **Template Method** — all pipeline orchestration belongs in the abstract base classes (`AbstractAddressValidationService`, `AbstractAuthenticationService`, `AbstractValidator`). Concrete provider classes fill in abstract members; they never re-implement orchestration.
- **Port/Adapter** — every pipeline step is expressed as a narrow interface (`IApiRequestAdapter`, `IApiRequestMapper`, `IApiResponseMapper`, `IValidator<T>`). Concrete implementations depend on the interface, not on each other.
- **No runtime reflection** — use source-generated `JsonSerializerContext` per project. Add `[JsonSerializable]` attributes to the context when new contract types are introduced.
- **`FrozenSet` for read-only lookups** — use `FrozenSet<T>` for any set that is built once at startup and queried frequently (country lists, supported locales, etc.).
- **`HybridCache` for token caching** — never use `IMemoryCache` or `IDistributedCache` directly for authentication tokens. Cache keys must match `^[a-zA-Z0-9_\-:]+$` and be prefixed with `vs-ave-auth:`.

## Testing Requirements

- Every new public type or member must be covered by tests.
- Integration tests must use **WireMock.Net** with real JSON fixtures in a `Fixtures/` directory — they must never call live provider APIs.
- After adding or changing any public API, update the corresponding `ApiTests.PublicApi_HasNoBreakingChanges_Async.verified.txt` snapshot by running the tests with `VERIFY_UPDATE=1` (or equivalent Verify update flag) and committing the result.
- Use **TUnit** (not xUnit or NUnit). Assertions use **AwesomeAssertions**. Mocks use **NSubstitute**. Test data uses **AutoFixture**.
- All `await` calls in tests also require `ConfigureAwait` — the same rule applies in test projects.

## Error Messages and Localization

- All user-facing validation error messages must be defined in the project's `Resources/Resources.resx` file. Never hardcode error message strings in C# source.
- Do not add new `.resx` entries without documenting the new string's placeholder semantics and do-not-translate list in the pull request description.

## What NOT to Do

- Do not add `using var` or `var` anywhere.
- Do not use `Activator`, `MethodInfo`, `PropertyInfo`, or any other reflection type in non-test code.
- Do not use `IMemoryCache` or `IDistributedCache` for OAuth tokens.
- Do not reference one integration package from another — they are peer packages that share only `Visus.AddressValidation`.
- Do not add third-party (non-Microsoft) runtime NuGet dependencies to source projects under `src/` — consumers inherit the transitive package graph. Analyzer and build-only packages (`<PrivateAssets>all</PrivateAssets>`) are exempt. Test projects under `tests/` are also exempt.
- Do not modify `Directory.Build.props` or `Directory.Build.targets` without understanding the MSBuild property inheritance across all projects.
- Do not suppress analyzer warnings with `#pragma warning disable` or `[SuppressMessage]` without a code comment explaining why.
- Do not add a `<Version>` element to any `.csproj` — versioning is handled by the release pipeline.
