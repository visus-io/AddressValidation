---
title: Batch Validators | Custom Integration
uid: custom-batch-validators
---

## Batch Validators

Batch validation reuses the existing per-request [request validator](xref:custom-validators) unchanged; each request in the list is still validated individually before the batch call is made, and any request that fails is resolved to an [`EmptyAddressValidationResponse`](xref:Visus.AddressValidation.Models.EmptyAddressValidationResponse) without ever reaching the provider. Only the API response needs a dedicated batch validator, since it now describes many addresses instead of one.

Extend [`AbstractBatchValidator<T>`](xref:Visus.AddressValidation.Validation.AbstractBatchValidator`1). Unlike [`AbstractValidator<T>`](xref:Visus.AddressValidation.Validation.AbstractValidator`1), which produces a single shared `ISet<ValidationState>`, the batch validator produces one independent `ISet<ValidationState>` per item, indexed positionally.

```csharp
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchApiResponseValidator : AbstractBatchValidator<ApiResponse>
{
    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, IReadOnlyList<int> requestIndexes, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        if ( instance.ErrorResponse is not null )
        {
            BroadcastToAll(ValidationState.CreateError(instance.ErrorResponse.Message), results);
            return ValueTask.FromResult(false);
        }

        if ( instance.Results is null || instance.Results.Length != requestIndexes.Count )
        {
            BroadcastToAll(ValidationState.CreateError(Resources.Validation_Batch_ResolvedAddressCountMismatch, requestIndexes.Count, instance.Results?.Length ?? 0), results);
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(true);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, IReadOnlyList<int> requestIndexes, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        for ( int i = 0; i < instance.Results!.Length; i++ )
        {
            if ( instance.Results[i].SuiteNumberInvalid )
            {
                results[i].Add(ValidationState.CreateError(Resources.Validation_Verification_RowValueCouldNotBeVerified, requestIndexes[i], nameof(AbstractAddressValidationRequest.AddressLines)));
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void BroadcastToAll(ValidationState state, IReadOnlyList<ISet<ValidationState>> results)
    {
        foreach ( ISet<ValidationState> itemResults in results )
        {
            itemResults.Add(state);
        }
    }
}
```

> [!IMPORTANT]
> `requestIndexes[i]` and `results[i]` are **not** the same kind of index. `results[i]` is positional within the *sent* batch: item `i` of `results` always corresponds to item `i` of the provider's response array (`instance.Results[i]` above), because locally-invalid requests were already filtered out before the API call. `requestIndexes[i]` is the *original* index that item held in the caller's input list, before any filtering. Use `results[i]` to add validation state for that item, and `requestIndexes[i]` only when the error message needs to reference the request's original position (for example, "Row 3 could not be verified" when the caller submitted 5 requests but only 3 were locally valid and sent).
>
> Conflating the two produces error messages that point at the wrong row whenever any request earlier in the batch fails local validation and gets filtered out before the sent batch is built.

> [!NOTE]
> `PreValidateAsync` controls whether `ValidateAsync` runs, same as the non-batch [Response Validator](xref:custom-validators#response-validator). Use it for batch-wide conditions, such as a top-level error payload or a result count that does not match the number of items sent, and broadcast the resulting `ValidationState` to every item's result set with a small helper like `BroadcastToAll` above.

> [!NOTE]
> Batch-level information that cannot be attributed to one specific address, such as a response-wide warning alert, should also be broadcast to every item's result set in `ValidateAsync` rather than assigned arbitrarily to a single item.

> [!IMPORTANT]
> `ExecuteAsync` must return exactly one [`IValidationResult`](xref:Visus.AddressValidation.Validation.IValidationResult) per item in `requestIndexes`. Extending `AbstractBatchValidator<T>` through `PreValidateAsync`/`ValidateAsync` as shown above guarantees this automatically. If a custom implementation violates the count contract, `ValidateManyAsync` throws `InvalidImplementationException` rather than silently misaligning results.

[!INCLUDE [internal-validation-note](../../includes/internal-validation-note.md)]
