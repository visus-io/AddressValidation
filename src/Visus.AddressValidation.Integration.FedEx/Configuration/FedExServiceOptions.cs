namespace Visus.AddressValidation.Integration.FedEx.Configuration;

using System.ComponentModel.DataAnnotations;
using AddressValidation.Abstractions;

/// <summary>
///     Configuration options for the FedEx address validation service.
/// </summary>
public sealed class FedExServiceOptions : IValidatableObject
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:FedEx";

    /// <summary>
    ///     Gets the base URI of the FedEx API endpoint, derived from the
    ///     current <see cref="ClientEnvironment" /> value.
    /// </summary>
    public Uri EndpointBaseUri =>
        ClientEnvironment switch
        {
            ClientEnvironment.DEVELOPMENT => Constants.DevelopmentEndpointBaseUri,
            ClientEnvironment.PRODUCTION => Constants.ProductionEndpointBaseUri,
            ClientEnvironment.SANDBOX => EndpointOverrideUri!,
            _ => Constants.DevelopmentEndpointBaseUri,
        };

    /// <summary>
    ///     Gets or sets the FedEx account number used to authenticate API
    ///     requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string AccountNumber { get; set; }

    /// <summary>
    ///     Gets or sets the target client environment, which determines
    ///     whether requests are sent to the FedEx sandbox or production
    ///     endpoint. Defaults to
    ///     <see cref="AddressValidation.Abstractions.ClientEnvironment.DEVELOPMENT" />.
    /// </summary>
    public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

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
    ///     Gets or sets a URI that overrides the default endpoint derived from
    ///     <see cref="ClientEnvironment" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is <b>required</b> when
    ///         <see cref="ClientEnvironment" /> is
    ///         <see cref="ClientEnvironment.SANDBOX" />; validation will fail
    ///         if it is <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         For all other environments this property is optional and, when
    ///         set, has no effect — the endpoint is always resolved from
    ///         <see cref="ClientEnvironment" />.
    ///     </para>
    /// </remarks>
    public Uri? EndpointOverrideUri { get; set; }

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
    ///     Validates that <see cref="EndpointOverrideUri" /> is not
    ///     <see langword="null" /> when <see cref="ClientEnvironment" /> is
    ///     <see cref="ClientEnvironment.SANDBOX" />, since the
    ///     sandbox environment requires an explicit endpoint to target a local
    ///     mock server.
    /// </remarks>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ( ClientEnvironment == ClientEnvironment.SANDBOX && EndpointOverrideUri is null )
        {
            yield return new ValidationResult(
                $"{nameof(EndpointOverrideUri)} must be set when {nameof(ClientEnvironment)} is {nameof(ClientEnvironment.SANDBOX)}.",
                [nameof(EndpointOverrideUri),]);
        }
    }
}
