namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Diagnostics;
using Abstractions;
using AddressValidation.Validation;
using Contracts;
using Resources;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class BatchApiResponseValidator : AbstractBatchValidator<ApiResponse>
{
    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, IReadOnlyList<int> requestIndexes, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        if ( instance.ErrorResponse is not null )
        {
            BroadcastErrorResponse(instance.ErrorResponse, results);
            return ValueTask.FromResult(false);
        }

        if ( instance.Result is null || instance.Result.ResolvedAddresses.Length != requestIndexes.Count )
        {
            ValidationState state = ValidationState.CreateError(
                Resources.Validation_Batch_ResolvedAddressCountMismatch,
                requestIndexes.Count,
                instance.Result?.ResolvedAddresses.Length ?? 0);

            BroadcastToAll(state, results);
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(true);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, IReadOnlyList<int> requestIndexes, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance.Result is not null);

        for ( int i = 0; i < instance.Result.ResolvedAddresses.Length; i++ )
        {
            ResolvedAddressValidator.Validate(instance.Result.ResolvedAddresses[i], requestIndexes[i], results[i]);
        }

        // Alerts are batch/response-level, not per-address, so there is no way to correlate them to one
        // specific request. Every WARNING alert is broadcast into every item's result set.
        foreach ( ApiResponse.Alert alert in instance.Result.Alerts )
        {
            if ( alert.AlertType != AlertType.WARNING )
            {
                continue;
            }

            ValidationState state = string.IsNullOrWhiteSpace(alert.Message)
                                        ? ValidationState.CreateWarning(alert.Code)
                                        : ValidationState.CreateWarning($"{alert.Code}: {alert.Message}");

            BroadcastToAll(state, results);
        }

        return ValueTask.CompletedTask;
    }

    private static void BroadcastErrorResponse(ApiErrorResponse errorResponse, IReadOnlyList<ISet<ValidationState>> results)
    {
        foreach ( ApiErrorResponse.Error error in errorResponse.Errors )
        {
            ValidationState state = string.IsNullOrWhiteSpace(error.Message)
                                        ? ValidationState.CreateError(error.Code)
                                        : ValidationState.CreateError($"{error.Code}: {error.Message}");

            BroadcastToAll(state, results);
        }
    }

    private static void BroadcastToAll(ValidationState state, IReadOnlyList<ISet<ValidationState>> results)
    {
        foreach ( ISet<ValidationState> itemResults in results )
        {
            itemResults.Add(state);
        }
    }
}
