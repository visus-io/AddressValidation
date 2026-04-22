namespace Visus.AddressValidation.Integration.PitneyBowes.Model;

using AddressValidation.Model;
using JetBrains.Annotations;

/// <summary>
///     Representation of a uniformed address validation request to Pitney Bowes
/// </summary>
[UsedImplicitly]
public sealed class PitneyBowesAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Indicates whether to include suggested address as part of the request.
    /// </summary>
    /// <remarks>
    ///     By default, this value is <see langword="false" />, it should only be set to <see langword="true" /> if the
    ///     initial request fails.
    /// </remarks>
    public bool IncludeSuggestions { get; set; }
}
