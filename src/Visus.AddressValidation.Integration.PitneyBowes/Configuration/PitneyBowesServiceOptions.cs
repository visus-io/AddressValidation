namespace Visus.AddressValidation.Integration.PitneyBowes.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration options for the Pitney Bowes address validation service.
/// </summary>
public sealed class PitneyBowesServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:PitneyBowes";

    /// <summary>
    ///     Gets the base URI of the Pitney Bowes API endpoint, derived from the
    ///     current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public Uri EndpointBaseUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

    /// <summary>
    ///     Gets or sets the API key issued by Pitney Bowes for the registered
    ///     application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ApiKey { get; set; }

    /// <summary>
    ///     Gets or sets the API secret issued by Pitney Bowes for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ApiSecret { get; set; }

    /// <summary>
    ///     Gets or sets the target client environment, which determines
    ///     whether requests are sent to the Pitney Bowes sandbox or production
    ///     endpoint. Defaults to
    ///     <see cref="AddressValidation.Abstractions.ClientEnvironment.DEVELOPMENT" />.
    /// </summary>
    public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

    /// <summary>
    ///     Gets or sets the Pitney Bowes developer ID associated with the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string DeveloperId { get; set; }
}
