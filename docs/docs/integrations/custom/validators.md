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

[!INCLUDE [internal-validation-note](../../includes/internal-validation-note.md)]
