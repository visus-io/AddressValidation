namespace Visus.AddressValidation.Models;

using Validation;

/// <summary>
///     Represents an empty validation response.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class EmptyAddressValidationResponse : AbstractAddressValidationResponse
{
    /// <summary>
    ///     Initializes a new instance of <see cref="EmptyAddressValidationResponse" />.
    /// </summary>
    /// <param name="validationResult">
    ///     The current <see cref="IValidationResult" />, or <see langword="null" /> if no validation ran.
    /// </param>
    public EmptyAddressValidationResponse(IValidationResult? validationResult = null)
        : base(validationResult)
    {
    }
}
