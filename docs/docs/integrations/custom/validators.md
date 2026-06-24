---
title: Validators | Custom Integration
uid: custom-validators
---

## Validators

Two validators are required: one for the incoming [request](xref:custom-models) and one for the provider's [API response](xref:custom-models). Both run inside the validation pipeline managed by [`AbstractAddressValidationService<TRequest, TApiResponse>`](xref:Visus.AddressValidation.Services.AbstractAddressValidationService`2) — the request validator runs before the API call, and the response validator runs after.

## Request Validator

Extend [`AbstractAddressValidationRequestValidator<TRequest>`](xref:Visus.AddressValidation.Validation.AbstractAddressValidationRequestValidator`1) and implement the two required abstract members. Override `PreValidateAsync` for provider-specific checks, but always call `base.PreValidateAsync` first and return `false` if it does.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class AddressValidationRequestValidator : AbstractAddressValidationRequestValidator<MyAddressValidationRequest>
{
    protected override string ProviderName => "My Provider";

    protected override FrozenSet<CountryCode> SupportedCountries => Constants.SupportedCountries;
}
```

> [!NOTE]
> `ProviderName` is the human-readable name of the provider. It appears in validation error messages when the request's `Country` is absent or not in `SupportedCountries` — for example, *"Country 'XX' is not supported by My Provider"*.

> [!NOTE]
> `SupportedCountries` is the [`FrozenSet<CountryCode>`](xref:Visus.AddressValidation.Abstractions.CountryCode) checked by the base `PreValidateAsync`. If `Country` is `null` or not in this set, an error is added to `results` and the pipeline short-circuits before `ValidateAsync` runs. By convention, define it in a static `Constants` class within the integration:
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

The inherited `ValidateAsync` validates the remaining address fields after the country check passes: `AddressLines` must be non-empty and contain at most 3 lines; `CityOrTown`, `StateOrProvince`, and `PostalCode` must be present (with country-specific exceptions for city-states and countries without postal codes).

To add provider-specific pre-validation — such as enforcing field ranges or environment-specific restrictions — override `PreValidateAsync` and always call `base.PreValidateAsync` first:

```csharp
protected override async ValueTask<bool> PreValidateAsync(MyAddressValidationRequest instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
{
    if (!await base.PreValidateAsync(instance, results, cancellationToken).ConfigureAwait(false))
    {
        return false;
    }

    // Provider-specific checks here.
    return true;
}
```

> [!IMPORTANT]
> The request validator **must** derive from `AbstractAddressValidationRequestValidator<TRequest>`. Passing a plain [`AbstractValidator<TRequest>`](xref:Visus.AddressValidation.Validation.AbstractValidator`1) subclass that does not go through this hierarchy will throw [`InvalidImplementationException`](xref:Visus.AddressValidation.InvalidImplementationException) at construction time.

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
            if ( string.IsNullOrWhiteSpace(error.Message) )
            {
                continue;
            }

            results.Add(string.IsNullOrWhiteSpace(error.Code)
                            ? ValidationState.CreateError(error.Message)
                            : ValidationState.CreateError($"{error.Code}: {error.Message}"));
        }

        return ValueTask.FromResult(false);
    }
}
```

> [!NOTE]
> `PreValidateAsync` controls whether `ValidateAsync` runs. Return `false` to short-circuit — errors already added to `results` will surface in `IAddressValidationResponse.Errors`, and `ValidateAsync` is not called. Return `true` to proceed to `ValidateAsync` for field-level checks on a structurally-valid response.

> [!NOTE]
> Override `ValidateAsync` for field-level validation that only applies once the response is structurally valid. Use [`ValidationState.CreateWarning`](xref:Visus.AddressValidation.Validation.ValidationState.CreateWarning*) for non-fatal conditions — warnings surface in `IAddressValidationResponse.Warnings` rather than `Errors`.

[!INCLUDE [internal-validation-note](../../includes/internal-validation-note.md)]
