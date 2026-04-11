namespace Visus.AddressValidation.Integration.PitneyBowes;

using System.Collections.Frozen;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration Constants for Pitney Bowes Service Integration
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Pitney Bowes Development Endpoint
    /// </summary>
    public static readonly Uri DevelopmentEndpointBaseUri = new("https://shipping-api-sandbox.pitneybowes.com/");

    /// <summary>
    ///     Pitney Bowes Production Endpoint
    /// </summary>
    public static readonly Uri ProductionEndpointBaseUri = new("https://shipping-api.pitneybowes.com/");

    /// <summary>
    ///     Countries that are supported by the Pitney Bowes Address Validation API.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries = new HashSet<CountryCode>
    {
        CountryCode.US,
    }.ToFrozenSet();
}
