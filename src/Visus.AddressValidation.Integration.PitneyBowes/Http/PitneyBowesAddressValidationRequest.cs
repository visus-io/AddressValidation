namespace Visus.AddressValidation.Integration.PitneyBowes.Http;

using System.Text.Json.Serialization;
using AddressValidation.Http;
using Serialization.Json;

/// <summary>
///     Representation of a uniformed address validation request to Pitney Bowes
/// </summary>
[JsonConverter(typeof(AddressValidationRequestConverter))]
public sealed class PitneyBowesAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Indicates whether to include suggested address as part of the request.
    /// </summary>
    /// <remarks>By default, this value is <c>false</c>, it should only be set to <c>true</c> if the initial request fails.</remarks>
    public bool IncludeSuggestions { get; set; }
}
