namespace Visus.AddressValidation.Integration.Ups;

using System.Collections.Frozen;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for UPS Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     UPS API Development Endpoint
    /// </summary>
    public static readonly Uri DevelopmentEndpointBaseUri = new("https://wwwcie.ups.com");

    /// <summary>
    ///     UPS API Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://onlinetools.ups.com");

    /// <summary>
    ///     Countries that are supported by the UPS Address Validation API.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.US,
        CountryCode.PR,
    }.ToFrozenSet();
}
