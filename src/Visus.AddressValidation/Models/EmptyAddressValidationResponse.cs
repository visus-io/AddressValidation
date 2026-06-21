namespace Visus.AddressValidation.Models;

using Validation;

/// <summary>
///     Represents an empty validation response
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class EmptyAddressValidationResponse : AbstractAddressValidationResponse
{
    /// <summary>
    ///     Initializes a new instance of <see cref="EmptyAddressValidationResponse" />.
    /// </summary>
    /// <param name="validationResult">
    ///     Current validation state of the response represented as an instance of
    ///     <see cref="IValidationResult" />.
    /// </param>
    public EmptyAddressValidationResponse(IValidationResult? validationResult = null)
        : base(validationResult)
    {
    }
}
