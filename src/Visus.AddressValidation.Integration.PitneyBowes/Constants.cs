namespace Visus.AddressValidation.Integration.PitneyBowes;

using AddressValidation.Abstractions;

/// <summary>
///     Configuration constants for the Pitney Bowes integration.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The Pitney Bowes API development endpoint.
    /// </summary>
    public static readonly Uri DevelopmentEndpointUri = new("https://shipping-api-sandbox.pitneybowes.com/");

    /// <summary>
    ///     The Pitney Bowes API production endpoint.
    /// </summary>
    public static readonly Uri ProductionEndpointUri = new("https://shipping-api.pitneybowes.com/");

    /// <summary>
    ///     The countries the Pitney Bowes Address Validation API supports.
    /// </summary>
    public static readonly FrozenSet<CountryCode> SupportedCountries =
    [
        CountryCode.US,
    ];
}
