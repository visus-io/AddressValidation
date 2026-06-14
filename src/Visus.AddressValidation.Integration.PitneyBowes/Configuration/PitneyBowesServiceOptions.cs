namespace Visus.AddressValidation.Integration.PitneyBowes.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using AddressValidation.Configuration;

/// <summary>
///     Configuration options for the Pitney Bowes address validation service.
/// </summary>
public sealed class PitneyBowesServiceOptions : AbstractServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:PitneyBowes";

    /// <inheritdoc />
    public override Uri EndpointUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointUri,
            ClientEnvironment.SANDBOX => EndpointUriOverride!,
            _ => Constants.DevelopmentEndpointUri,
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
    ///     Gets or sets the Pitney Bowes developer ID associated with the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string DeveloperId { get; set; }
}
