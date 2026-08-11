namespace Visus.AddressValidation.Integration.PitneyBowes.Model;

using JetBrains.Annotations;
using Models;

/// <summary>
///     Represents a unified address validation request sent to Pitney Bowes.
/// </summary>
[UsedImplicitly]
public sealed class PitneyBowesAddressValidationRequest : AbstractAddressValidationRequest
{
    /// <summary>
    ///     Indicates whether to include suggested addresses as part of the request.
    /// </summary>
    /// <remarks>
    ///     The default value is <see langword="false" />. Set it to <see langword="true" /> only if the initial
    ///     request fails.
    /// </remarks>
    public bool IncludeSuggestions { get; set; }
}
