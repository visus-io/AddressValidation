namespace Visus.AddressValidation.Integration.FedEx.Http;

using System.Text.Json.Serialization;
using AddressValidation.Http;
using Serialization.Json;

/// <summary>
///     Representation of a uniformed address validation request to FedEx.
/// </summary>
[JsonConverter(typeof(AddressValidationRequestConverter))]
public sealed class FedExAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Gets or sets the Client Reference ID
    /// </summary>
    public string? ClientReferenceId { get; set; }
}
