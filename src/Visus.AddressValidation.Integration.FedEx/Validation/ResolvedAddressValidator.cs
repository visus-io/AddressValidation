namespace Visus.AddressValidation.Integration.FedEx.Validation;

using AddressValidation.Abstractions;
using AddressValidation.Models;
using AddressValidation.Resources;
using AddressValidation.Validation;
using Contracts;

internal static class ResolvedAddressValidator
{
    internal static void Validate(ApiResponse.ResolvedAddress address, int index, ISet<ValidationState> results)
    {
        if ( address.Attributes.InvalidSuiteNumber )
        {
            const string propertyName = nameof(address.StreetLinesToken);
            results.Add(ValidationState.CreateError(Resources.Validation_Verification_RowValueCouldNotBeVerified, index,
                propertyName,
                FedEx.Resources.Resources.Validation_FedEx_InvalidSuiteNumber));
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
                FedEx.Resources.Resources.Validation_FedEx_SuiteNumberNotProvided));
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
