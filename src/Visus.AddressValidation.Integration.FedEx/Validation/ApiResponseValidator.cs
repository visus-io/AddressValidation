namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Diagnostics;
using AddressValidation.Abstractions;
using AddressValidation.Model;
using AddressValidation.Validation;
using Http;
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
            ApiResponse.ResolvedAddress address = instance.Result.ResolvedAddresses[i];

            if ( address.Attributes.InvalidSuiteNumber )
            {
                const string propertyName = nameof(address.StreetLinesToken);
                results.Add(ValidationState.CreateError(Resources.Validation_Verification_RowValueCouldNotBeVerified, i,
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
                results.Add(ValidationState.CreateWarning(Resources.Validation_Verification_RowValueCouldNotBeVerified, i,
                    propertyName,
                    "Suite number was not provided in the request."));
            }
        }

        return ValueTask.CompletedTask;
    }
}
