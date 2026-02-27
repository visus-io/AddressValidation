namespace Visus.AddressValidation.Validation;

using System.Diagnostics;
using Http;

/// <summary>
///     Base Validator for <see cref="AbstractAddressValidationRequest" /> instances.
/// </summary>
public abstract class AbstractAddressValidationRequestValidator<T> : AbstractValidator<T>
    where T : AbstractAddressValidationRequest
{
    /// <inheritdoc />
    protected override ValueTask<bool> PreValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance != null, nameof(instance) + " != null");
        Debug.Assert(results != null, nameof(results) + " != null");

        if ( instance.Country is not null )
        {
            return ValueTask.FromResult(true);
        }

        results.Add(ValidationState.CreateError("Value cannot be null.", nameof(instance.Country)));

        return ValueTask.FromResult(false);
    }

    /// <inheritdoc />
    protected override ValueTask ValidateAsync(T instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
    {
        Debug.Assert(instance != null, nameof(instance) + " != null");
        Debug.Assert(results != null, nameof(results) + " != null");

        switch ( instance.AddressLines.Count )
        {
            case 0:
            case > 0 when instance.AddressLines.All(string.IsNullOrWhiteSpace):
                results.Add(ValidationState.CreateError("Cannot be null or empty.", nameof(instance.AddressLines)));
                break;
            case > 3:
                results.Add(ValidationState.CreateError("Cannot contain more than 3 entries.", nameof(instance.AddressLines)));
                break;
        }

        if ( !Constants.CityStates.Contains(instance.Country!.Value) )
        {
            if ( string.IsNullOrWhiteSpace(instance.CityOrTown) )
            {
                results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.CityOrTown)));
            }

            if ( string.IsNullOrWhiteSpace(instance.StateOrProvince) )
            {
                results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.StateOrProvince)));
            }
        }

        if ( !Constants.NoPostalCode.Contains(instance.Country!.Value) && string.IsNullOrWhiteSpace(instance.PostalCode) )
        {
            results.Add(ValidationState.CreateError("Value cannot be null or empty.", nameof(instance.PostalCode)));
        }

        if ( Constants.NoPostalCode.Contains(instance.Country!.Value) )
        {
            results.Add(ValidationState.CreateError("{0}: {1} is currently not supported for address validation.", nameof(instance.Country), instance.Country));
        }

        return base.ValidateAsync(instance, results, cancellationToken);
    }
}
