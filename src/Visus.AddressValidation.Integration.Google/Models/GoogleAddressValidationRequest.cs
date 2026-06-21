namespace Visus.AddressValidation.Integration.Google.Models;

using AddressValidation.Abstractions;
using AddressValidation.Models;
using JetBrains.Annotations;

/// <summary>
///     Representation of a unified address validation request to Google.
/// </summary>
[UsedImplicitly]
public sealed class GoogleAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets a value indicating whether USPS CASS processing is enabled for this request.
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
