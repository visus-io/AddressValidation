namespace Visus.AddressValidation.Models;

/// <summary>
///     Provides equality comparison for <see cref="AbstractAddressValidationResponse" /> instances.
/// </summary>
public sealed class AddressValidationResponseEqualityComparer : IEqualityComparer<AbstractAddressValidationResponse>
{
    private AddressValidationResponseEqualityComparer()
    {
    }

    /// <summary>
    ///     Gets the default instance of <see cref="AddressValidationResponseEqualityComparer" />.
    /// </summary>
    public static AddressValidationResponseEqualityComparer Default { get; } = new();

    /// <inheritdoc />
    public bool Equals(AbstractAddressValidationResponse? x, AbstractAddressValidationResponse? y)
    {
        if ( ReferenceEquals(x, y) )
        {
            return true;
        }

        if ( x is null || y is null || x.GetType() != y.GetType() )
        {
            return false;
        }

        return x.AddressLines.SetEquals(y.AddressLines)
            && string.Equals(x.CityOrTown, y.CityOrTown, StringComparison.OrdinalIgnoreCase)
            && x.Country == y.Country
            && x.IsResidential == y.IsResidential
            && string.Equals(x.PostalCode, y.PostalCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.StateOrProvince, y.StateOrProvince, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public int GetHashCode(AbstractAddressValidationResponse obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return obj.GetHashCode();
    }
}
