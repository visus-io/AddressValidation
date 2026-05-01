namespace Visus.AddressValidation.Models;

using System.Collections.Frozen;
using Abstractions;
using Validation;

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationResponse" />.
/// </summary>
public abstract class AbstractAddressValidationResponse : IAddressValidationResponse
{
    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractAddressValidationResponse" />.
    /// </summary>
    /// <param name="validationResult">
    ///     The current validation state of the response, or <see langword="null" /> if no validation was performed.
    /// </param>
    protected AbstractAddressValidationResponse(IValidationResult? validationResult = null)
    {
        if ( validationResult is null )
        {
            return;
        }

        Errors = validationResult.Errors
                                 .Select(s => s.Message)
                                 .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        Warnings = validationResult.Warnings
                                   .Select(s => s.Message)
                                   .ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public IReadOnlySet<string> Errors { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public IReadOnlySet<string> Warnings { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public IReadOnlySet<string> AddressLines { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public string? CityOrTown { get; init; }

    /// <inheritdoc />
    public CountryCode Country { get; init; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> CustomResponseData { get; init; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
           .AsReadOnly();

    /// <inheritdoc />
    public bool? IsResidential { get; init; }

    /// <inheritdoc />
    public string? PostalCode { get; init; }

    /// <inheritdoc />
    public string? StateOrProvince { get; init; }

    /// <inheritdoc />
    public IReadOnlyList<IAddressValidationResponse> Suggestions { get; init; } = [];

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is AbstractAddressValidationResponse other
            && AddressValidationResponseEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        int addressLinesHash = AddressLines.Aggregate(
            0,
            (current, addressLine) => current ^ StringComparer.OrdinalIgnoreCase.GetHashCode(addressLine)
        );

        HashCode hashCode = new();
        hashCode.Add(addressLinesHash);
        hashCode.Add(CityOrTown, StringComparer.OrdinalIgnoreCase);
        hashCode.Add((int)Country);
        hashCode.Add(IsResidential);
        hashCode.Add(PostalCode, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(StateOrProvince, StringComparer.OrdinalIgnoreCase);

        return hashCode.ToHashCode();
    }
}

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationResponse" /> backed by a specific API response type.
/// </summary>
/// <typeparam name="TResponse">
///     The type of the underlying API response.
/// </typeparam>
public abstract class AbstractAddressValidationResponse<TResponse>
    : AbstractAddressValidationResponse where TResponse : class
{
    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractAddressValidationResponse{TResponse}" />.
    /// </summary>
    /// <param name="response">The underlying API response returned by the address validation service.</param>
    /// <param name="validationResult">
    ///     The current validation state of the response, or <see langword="null" /> if no validation was performed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="response" /> is <see langword="null" />.
    /// </exception>
    protected AbstractAddressValidationResponse(TResponse response, IValidationResult? validationResult = null) : base(validationResult)
    {
        ArgumentNullException.ThrowIfNull(response);
    }
}
