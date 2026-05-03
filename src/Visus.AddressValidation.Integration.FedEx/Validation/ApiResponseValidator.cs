namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Diagnostics;
using Abstractions;
using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Validation;
using Contracts;
using Resources;

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
            ValidateResolvedAddress(instance.Result.ResolvedAddresses[i], i, results);
        }

        foreach ( ApiResponse.Alert alert in instance.Result.Alerts )
        {
            switch (alert.AlertType)
            {
                case AlertType.WARNING:
                    results.Add(string.IsNullOrWhiteSpace(alert.Message)
                                    ? ValidationState.CreateWarning(alert.Code)
                                    : ValidationState.CreateWarning($"{alert.Code}: {alert.Message}"));
                    break;
                case AlertType.ERROR:
                    results.Add(string.IsNullOrWhiteSpace(alert.Message)
                                    ? ValidationState.CreateError(alert.Code)
                                    : ValidationState.CreateError($"{alert.Code}: {alert.Message}"));
                    break;
                case AlertType.NOTE:
                default:
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void ValidateResolvedAddress(ApiResponse.ResolvedAddress address, int index, ISet<ValidationState> results)
    {
        if ( address.Attributes.InvalidSuiteNumber )
        {
            const string propertyName = nameof(address.StreetLinesToken);
            results.Add(ValidationState.CreateError(Resources.Validation_Verification_RowValueCouldNotBeVerified, index,
                propertyName,
                "Invalid suite number was provided in the request."));
        }

        if ( !address.Attributes.IsValidStreetAddress && address.CountryCode != CountryCode.US )
        {
            const string propertyName = nameof(AbstractAddressValidationRequest.AddressLines);
            results.Add(ValidationState.CreateWarning(Resources.Validation_Verification_ValueCouldNotBeVerified, propertyName));
        }

        if ( !address.Attributes.IsValidPostalCode && address.CountryCode == CountryCode.US )
        {
            const string propertyName = nameof(AbstractAddressValidationRequest.PostalCode);
            results.Add(ValidationState.CreateError(Resources.Validation_Verification_ValueCouldNotBeVerified, propertyName));
        }

        if ( address.Attributes.SuiteRequiredButMissing )
        {
            const string propertyName = nameof(address.StreetLinesToken);
            results.Add(ValidationState.CreateWarning(Resources.Validation_Verification_RowValueCouldNotBeVerified, index,
                propertyName,
                "Suite number was not provided in the request."));
        }

        if ( address.CustomerMessages is not { Length: > 0, } customerMessages )
        {
            return;
        }

        foreach ( ApiResponse.CustomerMessage message in customerMessages )
        {
            results.Add(string.IsNullOrWhiteSpace(message.Message)
                            ? ValidationState.CreateWarning(message.Code)
                            : ValidationState.CreateWarning($"{message.Code}: {message.Message}"));
        }
    }
}
