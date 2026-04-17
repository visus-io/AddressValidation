namespace Visus.AddressValidation.Integration.Google.Http;

using AddressValidation.Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using JetBrains.Annotations;

/// <summary>
///     Representation of a uniformed address validation request to Google.
/// </summary>
[UsedImplicitly]
public sealed class GoogleAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets if USPS CASS processing is supported
    /// </summary>
    /// <remarks>Currently, USPS CASS is only supported for the <see cref="CountryCode.US" />.</remarks>
    public bool EnableUspsCass => Country == CountryCode.US;

    /// <summary>
    ///     Gets or sets the Previous Response ID
    /// </summary>
    /// <remarks>
    ///     Value should only be set if an address requires re-validation. The value can be retrieved from the previous request
    ///     within the
    ///     <see cref="IAddressValidationResponse.CustomResponseData" /> collection with the key <c>responseId</c>.
    /// </remarks>
    public Guid? PreviousResponseId { get; set; }
}
