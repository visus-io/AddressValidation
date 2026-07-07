## Description

<!-- Clear and concise description of your changes -->

## Type of Change

- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] New provider integration
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Refactoring (no functional changes)
- [ ] Dependency update
- [ ] Localization / translation

## Related Issues

Closes #
Related to #

## Changes Made

-
-
-

## Testing

- [ ] Added / updated unit tests
- [ ] Added / updated integration tests (WireMock fixtures updated if needed)
- [ ] Public API snapshot updated (`ApiTests.PublicApi_HasNoBreakingChanges_Async.verified.txt`)
- [ ] All tests pass (`dotnet test`)

**Test environment:** .NET

## Code Quality

- [ ] No `var` — explicit types throughout
- [ ] `ConfigureAwait` on every `await` (including tests)
- [ ] No reflection (`Activator`, `MethodInfo`, `PropertyInfo`) in non-test code
- [ ] No `IMemoryCache` / `IDistributedCache` used for OAuth tokens
- [ ] New JSON contract types registered with `[JsonSerializable]` in the project's `JsonSerializerContext`
- [ ] New public members carry XML doc comments

## Documentation

- [ ] New `.resx` strings: placeholder semantics and do-not-translate tokens documented in this PR description
- [ ] `ARCHITECTURE.md` updated (if structure or patterns changed)

## Breaking Changes

<!-- Complete this section only if "Breaking change" is checked above -->

### Impact

### Migration

```csharp
// Before

// After
```

## Additional Notes

## Checklist

- [ ] I have performed a self-review of my own code
- [ ] My changes generate no new warnings or errors (`dotnet build` is clean)
- [ ] Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/)
