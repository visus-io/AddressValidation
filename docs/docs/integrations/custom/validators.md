---
title: Validators | Custom Integration
uid: custom-validators
---

## Validators

Provide two validators: one for the incoming [request](xref:custom-models) and one for the provider's [API response](xref:custom-models). Both run inside the validation pipeline that [`AbstractAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractAddressValidationService`2) manages. The request validator runs before the API call. The response validator runs after.

## Request Validator

Extend [`AbstractAddressValidationRequestValidator<TRequest>`](xref:Visus.AddressValidation.Validation.AbstractAddressValidationRequestValidator`1) and implement the two required abstract members. The base class runs its shared country and address-field checks automatically, before your own checks run. Override `PreValidateAsync` to add provider-specific checks. You do not need to call `base.PreValidateAsync` — the base class's checks always run first, regardless of what your override does.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<MyAddressValidationRequest>
{
    protected override string ProviderName => "My Provider";

    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
```

> [!NOTE]
> `ProviderName` is the human-readable name of the provider. It appears in validation error messages when the request's `Country` is absent or not in `SupportedCountries`, for example *"Country 'XX' is not supported by My Provider"*.

> [!NOTE]
> `SupportedCountries` is the [`FrozenSet<CountryCode>`](xref:Visus.AddressValidation.Abstractions.CountryCode) checked by the base class's shared country validation. This check runs automatically, before your `PreValidateAsync` override. If `Country` is `null` or not in this set, an error is added to `results` and the pipeline short-circuits before `ValidateAsync` runs. By convention, define it in a static `Constants` class within the integration:
> ```csharp
> public static class Constants
> {
>     public static readonly FrozenSet<CountryCode> SupportedCountries =
>     [
>         CountryCode.US,
>         CountryCode.PR,
>     ];
> }
> ```

The base class checks the remaining address fields after the country check passes and after your `PreValidateAsync` override (if any) returns `true`: `AddressLines` must be non-empty and contain at most 3 lines; `CityOrTown`, `StateOrProvince`, and `PostalCode` must be present (with country-specific exceptions for city-states and countries without postal codes). This check runs independently of whether you override `ValidateAsync` — but if your `PreValidateAsync` returns `false`, this check is skipped, same as any other pre-validation failure.

To add provider-specific pre-validation (such as enforcing field ranges or environment-specific restrictions), override `PreValidateAsync`. The base class's shared country check already ran before your override runs, so add only your own checks:

```csharp
protected override ValueTask<bool> PreValidateAsync(MyAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
{
    // Provider-specific checks here.
    return ValueTask.FromResult(true);
}
```

> [!IMPORTANT]
> The request validator **must** derive from `AbstractAddressValidationRequestValidator<TRequest>`. A plain [`AbstractValidator<TRequest>`](xref:Visus.AddressValidation.Validation.AbstractValidator`1) subclass that skips this hierarchy throws [`InvalidImplementationException`](xref:Visus.AddressValidation.InvalidImplementationException) at construction time.

## Response Validator

Extend [`AbstractValidator<TApiResponse>`](xref:Visus.AddressValidation.Validation.AbstractValidator`1). Override `PreValidateAsync` to detect provider error payloads and short-circuit validation; any [`ValidationState`](xref:Visus.AddressValidation.Validation.ValidationState) errors added here surface in `IAddressValidationResponse.Errors`.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class ApiResponseValidator : AbstractValidator<ApiResponse>
{
    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( instance.Errors is null || instance.Errors.Length == 0 )
        {
            return ValueTask.FromResult(instance.Result is not null);
        }

        foreach ( ApiResponse.ErrorPayload error in instance.Errors )
        {
            results.Add(ApiMessageFormatter.CreateError(error.Code, error.Message));
        }

        return ValueTask.FromResult(false);
    }
}
```

> [!NOTE]
> `PreValidateAsync` controls whether `ValidateAsync` runs. Return `false` to short-circuit: errors already added to `results` surface in `IAddressValidationResponse.Errors`, and `ValidateAsync` does not run. Return `true` to proceed to `ValidateAsync` for field-level checks on a structurally-valid response.

> [!NOTE]
> [`ApiMessageFormatter`](xref:Visus.AddressValidation.Validation.ApiMessageFormatter) turns a provider's `(code, message)` pair into a `ValidationState`. Use `ApiMessageFormatter.CreateError` and `ApiMessageFormatter.CreateWarning` instead of writing the same code-or-message fallback in every provider. Both methods accept a `code` or `message` that is null, empty, or whitespace. Each method falls back to whichever value is present. Each method throws `ArgumentException` only when both `code` and `message` are null, empty, or whitespace.

> [!IMPORTANT]
> The sample loop above calls `ApiMessageFormatter.CreateError` without a guard. This is safe only when the provider always sends a code, a message, or both on every error entry. Confirm that guarantee against your provider's API contract first. If an entry can arrive with neither field set, skip it before the call:
> ```csharp
> foreach ( ApiResponse.ErrorPayload error in instance.Errors )
> {
>     if ( string.IsNullOrWhiteSpace(error.Code) && string.IsNullOrWhiteSpace(error.Message) )
>     {
>         continue;
>     }
>
>     results.Add(ApiMessageFormatter.CreateError(error.Code, error.Message));
> }
> ```

> [!NOTE]
> Override `ValidateAsync` for field-level validation that only applies once the response is structurally valid. Use [`ValidationState.CreateWarning`](xref:Visus.AddressValidation.Validation.ValidationState.CreateWarning*) for non-fatal conditions: warnings surface in `IAddressValidationResponse.Warnings`, instead of `Errors`.

[!INCLUDE [internal-validation-note](../../includes/internal-validation-note.md)]
