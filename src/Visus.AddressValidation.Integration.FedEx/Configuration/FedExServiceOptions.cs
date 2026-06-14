namespace Visus.AddressValidation.Integration.FedEx.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;
using AddressValidation.Configuration;

/// <summary>
///     Configuration options for the FedEx address validation service.
/// </summary>
public sealed class FedExServiceOptions : AbstractServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:FedEx";

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
    ///     Gets or sets the FedEx account number used to authenticate API
    ///     requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string AccountNumber { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client ID issued by FedEx for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 client secret issued by FedEx for the
    ///     registered application.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ClientSecret { get; set; }

    /// <summary>
    ///     Gets or sets the IETF BCP 47 language tag that identifies the locale
    ///     used for address validation responses returned by the FedEx API.
    /// </summary>
    /// <remarks>
    ///     When <see langword="null" /> (the default), the <c>x-locale</c>
    ///     request header is omitted and FedEx will use its own default locale.
    ///     Set to an IETF BCP 47 tag such as <c>en-US</c> or <c>fr-FR</c> to
    ///     request responses in a specific language and region.
    /// </remarks>
    public string? Locale { get; set; }

    /// <summary>
    ///     Performs cross-property validation on the options object.
    /// </summary>
    /// <param name="validationContext">
    ///     The context in which validation is performed.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="ValidationResult" /> instances describing
    ///     any validation failures, or an empty collection if the options are
    ///     valid.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Inherits the sandbox endpoint override check from
    ///         <see cref="AbstractServiceOptions.Validate" />.
    ///     </para>
    ///     <para>
    ///         Also validates that <see cref="Locale" />, when set, is one of
    ///         the IETF BCP 47 language tags listed in
    ///         <see cref="Constants.SupportedLocales" />.
    ///     </para>
    /// </remarks>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach ( ValidationResult result in base.Validate(validationContext) )
        {
            yield return result;
        }

        if ( Locale is not null && !Constants.SupportedLocales.Contains(Locale) )
        {
            yield return new ValidationResult(
                $"'{Locale}' is not a supported IETF BCP 47 language tag.",
                [nameof(Locale),]);
        }
    }
}
