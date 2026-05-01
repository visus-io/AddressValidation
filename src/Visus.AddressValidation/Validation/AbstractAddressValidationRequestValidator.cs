namespace Visus.AddressValidation.Validation;

using System.Collections.Frozen;
using System.Diagnostics;
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

    /// <summary>
    ///     Checks that <paramref name="instance" /> specifies a non-null country code supported by
    ///     <see cref="ProviderName" />. Validation is skipped if the country is missing or unsupported.
    /// </summary>
    /// <param name="instance">The request instance to pre-validate.</param>
    /// <param name="results">The set of <see cref="ValidationState" /> objects for the current instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     <see langword="true" /> when the country is present and supported; otherwise <see langword="false" />,
    ///     and an error is added to <paramref name="results" />.
    /// </returns>
    protected override ValueTask<bool> PreValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance != null);
        Debug.Assert(results != null);

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

    /// <summary>
    ///     Validates the address fields of <paramref name="instance" />, checking that address lines, city/town,
    ///     state/province, and postal code are present and within the expected constraints for the given country.
    /// </summary>
    /// <param name="instance">The request instance to validate.</param>
    /// <param name="results">The set of <see cref="ValidationState" /> objects for the current instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    protected override ValueTask ValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance != null);
        Debug.Assert(results != null);

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

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
