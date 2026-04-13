namespace Visus.AddressValidation.Integration.Google.Configuration;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Configuration options for the Google Address Validation API service
///     account.
/// </summary>
public sealed class GoogleServiceOptions
{
    /// <summary>
    ///     The configuration section path used to bind these options from
    ///     <c>appsettings.json</c> or other configuration sources.
    /// </summary>
    public const string SectionName = "AddressValidationSettings:Google";

    /// <summary>
    ///     Gets or sets the RSA private key associated with the service account,
    ///     used to sign JWT tokens for authentication.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string PrivateKey { get; set; }

    /// <summary>
    ///     Gets or sets the Google Cloud project ID that hosts the Address
    ///     Validation API.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ProjectId { get; set; }

    /// <summary>
    ///     Gets or sets the email address of the Google service account used to
    ///     authenticate API requests.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ServiceAccountEmail { get; set; }
}
