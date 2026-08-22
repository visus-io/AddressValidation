namespace Visus.AddressValidation.Validation;

using Abstractions;
using Models;
using Resources;

/// <summary>
///     Base Validator for <see cref="AbstractAddressValidationRequest" /> instances.
/// </summary>
public abstract class AbstractAddressValidationRequestValidator<T> : AbstractValidator<T>
    where T : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets the display name of the address validation provider.
    /// </summary>
    protected abstract string ProviderName { get; }

    /// <summary>
    ///     Gets the countries supported by the address validation provider.
    /// </summary>
    protected abstract FrozenSet<CountryCode> SupportedCountries { get; }

    internal sealed override ValueTask<bool> PreValidateInternalAsync(T instance, ISet<ValidationState> results)
    {
        if ( instance.Country is not null )
        {
            if ( SupportedCountries.Contains(instance.Country.Value) )
            {
                return ValueTask.FromResult(true);
            }

            results.Add(ValidationState.CreateError(Resources.Validation_Provider_CountryNotSupported, nameof(instance.Country), instance.Country, ProviderName));
            return ValueTask.FromResult(false);
        }

        results.Add(ValidationState.CreateError(Resources.Validation_Field_CannotBeNullOrEmpty, nameof(instance.Country)));

        return ValueTask.FromResult(false);
    }

    internal sealed override ValueTask ValidateInternalAsync(T instance, ISet<ValidationState> results)
    {
        switch ( instance.AddressLines.Count )
        {
            case 0:
            case > 0 when instance.AddressLines.All(string.IsNullOrWhiteSpace):
                results.Add(ValidationState.CreateError(Resources.Validation_Field_CannotBeNullOrEmpty, nameof(instance.AddressLines)));
                break;
            case > 3:
                results.Add(ValidationState.CreateError(Resources.Validation_Address_LinesCannotExceedThree, nameof(instance.AddressLines)));
                break;
        }

        if ( !Constants.CityStates.Contains(instance.Country!.Value) )
        {
            if ( string.IsNullOrWhiteSpace(instance.CityOrTown) )
            {
                results.Add(ValidationState.CreateError(Resources.Validation_Field_CannotBeNullOrEmpty, nameof(instance.CityOrTown)));
            }

            if ( string.IsNullOrWhiteSpace(instance.StateOrProvince) )
            {
                results.Add(ValidationState.CreateError(Resources.Validation_Field_CannotBeNullOrEmpty, nameof(instance.StateOrProvince)));
            }
        }

        if ( !Constants.NoPostalCode.Contains(instance.Country!.Value) && string.IsNullOrWhiteSpace(instance.PostalCode) )
        {
            results.Add(ValidationState.CreateError(Resources.Validation_Field_CannotBeNullOrEmpty, nameof(instance.PostalCode)));
        }

        if ( Constants.NoPostalCode.Contains(instance.Country!.Value) && string.IsNullOrWhiteSpace(instance.NoPostalCodeFallback) )
        {
            results.Add(ValidationState.CreateError(Resources.Validation_Address_CountryNotSupported, nameof(instance.Country), instance.Country));
        }

        return ValueTask.CompletedTask;
    }
}
