namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Diagnostics;
using Abstractions;
using AddressValidation.Validation;
using Contracts;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
internal sealed class ApiResponseValidator : AbstractValidator<ApiResponse>
{
    protected override ValueTask<bool> PreValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        if ( instance.ErrorResponse is null )
        {
            return ValueTask.FromResult(instance.Result is not null);
        }

        foreach ( ApiErrorResponse.Error error in instance.ErrorResponse.Errors )
        {
            results.Add(string.IsNullOrWhiteSpace(error.Message)
                            ? ValidationState.CreateError(error.Code)
                            : ValidationState.CreateError($"{error.Code}: {error.Message}"));
        }

        return ValueTask.FromResult(false);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance.Result is not null);

        for ( int i = 0; i < instance.Result.ResolvedAddresses.Length; i++ )
        {
            ResolvedAddressValidator.Validate(instance.Result.ResolvedAddresses[i], i, results);
        }

        foreach ( ApiResponse.Alert alert in instance.Result.Alerts )
        {
            switch ( alert.AlertType )
            {
                case AlertType.WARNING:
                    results.Add(string.IsNullOrWhiteSpace(alert.Message)
                                    ? ValidationState.CreateWarning(alert.Code)
                                    : ValidationState.CreateWarning($"{alert.Code}: {alert.Message}"));
                    break;
                case AlertType.NOTE:
                default:
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }
}
