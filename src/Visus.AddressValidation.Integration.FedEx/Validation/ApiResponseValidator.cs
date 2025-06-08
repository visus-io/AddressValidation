namespace Visus.AddressValidation.Integration.FedEx.Validation;

using System.Diagnostics;
using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Validation;
using Http;

internal sealed class ApiResponseValidator : AbstractApiResponseValidator<ApiResponse>
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
                            : ValidationState.CreateError("{0}: {1}", error.Code, error.Message));
        }

        return ValueTask.FromResult(false);
    }

    protected override ValueTask ValidateAsync(ApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance.Result != null, "instance.Result != null");

        for ( int i = 0; i < instance.Result.ResolvedAddresses.Length; i++ )
        {
            ApiResponse.ResolvedAddress address = instance.Result.ResolvedAddresses[i];

            if ( address.Attributes.InvalidSuiteNumber )
            {
                const string propertyName = nameof(address.StreetLinesToken);
                results.Add(ValidationState.CreateError("[Row {0}] {1}: {2}", i,
                                                        propertyName,
                                                        "Invalid suite number was provided in the request."));
            }

            if ( !address.Attributes.IsValidStreetAddress && address.CountryCode != CountryCode.US )
            {
                const string propertyName = nameof(AbstractAddressValidationRequest.AddressLines);
                results.Add(ValidationState.CreateWarning("Value could not be verified.", propertyName));
            }

            if ( !address.Attributes.IsValidPostalCode )
            {
                const string propertyName = nameof(AbstractAddressValidationRequest.PostalCode);
                results.Add(ValidationState.CreateError("Value could not be verified.", propertyName));
            }

            if ( address.Attributes.SuiteRequiredButMissing )
            {
                const string propertyName = nameof(address.StreetLinesToken);
                results.Add(ValidationState.CreateWarning("[Row {0}] {1}: {2}", i,
                                                          propertyName,
                                                          "Suite number was not provided in the request."));
            }
        }

        return ValueTask.CompletedTask;
    }
}
