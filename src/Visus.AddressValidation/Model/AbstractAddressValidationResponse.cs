namespace Visus.AddressValidation.Model;

using Abstractions;
using Http;
using Validation;

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationResponse" />.
/// </summary>
public abstract class AbstractAddressValidationResponse :
	IAddressValidationResponse,
	IEquatable<AbstractAddressValidationResponse>
{
	/// <summary>
	///     Initializes a new instance of the <see cref="AbstractAddressValidationResponse" />.
	/// </summary>
	/// <param name="validationResult">
	///     Current validation state of the response represented as an instance of
	///     <see cref="IValidationResult" />.
	/// </param>
	protected AbstractAddressValidationResponse(IValidationResult? validationResult = null)
	{
		if ( validationResult is null )
		{
			return;
		}

		Errors = validationResult.Errors
								 .Select(s => s.Message)
								 .ToHashSet(StringComparer.OrdinalIgnoreCase);

		Warnings = validationResult.Warnings
								   .Select(s => s.Message)
								   .ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	public IReadOnlySet<string> Errors { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc />
	public IReadOnlySet<string> Warnings { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc />
	public IReadOnlySet<string> AddressLines { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc />
	public string? CityOrTown { get; init; }

	/// <inheritdoc />
	public CountryCode Country { get; init; }

	/// <inheritdoc />
	public IReadOnlyDictionary<string, object?> CustomResponseData { get; init; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc />
	public bool? IsResidential { get; init; }

	/// <inheritdoc />
	public string? PostalCode { get; init; }

	/// <inheritdoc />
	public string? StateOrProvince { get; init; }

	/// <inheritdoc />
	public IReadOnlyList<IAddressValidationResponse> Suggestions { get; protected init; } = [];

	/// <summary>
	///     Indicates whether the values of the two specified <see cref="AbstractAddressValidationResponse" /> objects are
	///     equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns><c>true</c> if <c>left</c> and <c>right</c> are equal; otherwise, <c>false</c>.</returns>
	public static bool operator ==(AbstractAddressValidationResponse? left, AbstractAddressValidationResponse? right)
	{
		return Equals(left, right);
	}

	/// <summary>
	///     Indicates whether the values of the two specified <see cref="AbstractAddressValidationResponse" /> objects are not
	///     equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns><c>true</c> if <c>left</c> and <c>right</c> are not equal; otherwise, <c>false</c>.</returns>
	public static bool operator !=(AbstractAddressValidationResponse? left, AbstractAddressValidationResponse? right)
	{
		return !Equals(left, right);
	}

	/// <inheritdoc />
	public override bool Equals(object? obj)
	{
		return obj is AbstractAddressValidationResponse other && Equals(other);
	}

	/// <inheritdoc />
	public bool Equals(AbstractAddressValidationResponse? other)
	{
		if ( other is null )
		{
			return false;
		}

		return AddressLines.SequenceEqual(other.AddressLines, StringComparer.OrdinalIgnoreCase)
			&& string.Equals(CityOrTown, other.CityOrTown, StringComparison.OrdinalIgnoreCase)
			&& Country == other.Country
			&& IsResidential == other.IsResidential
			&& string.Equals(PostalCode, other.PostalCode, StringComparison.OrdinalIgnoreCase)
			&& string.Equals(StateOrProvince, other.StateOrProvince, StringComparison.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		HashCode hashCode = new();

		foreach ( string addressLine in AddressLines )
		{
			hashCode.Add(addressLine, StringComparer.OrdinalIgnoreCase);
		}

		hashCode.Add(CityOrTown, StringComparer.OrdinalIgnoreCase);
		hashCode.Add((int) Country);
		hashCode.Add(IsResidential);
		hashCode.Add(PostalCode, StringComparer.OrdinalIgnoreCase);
		hashCode.Add(StateOrProvince, StringComparer.OrdinalIgnoreCase);

		return hashCode.ToHashCode();
	}
}

/// <summary>
///     Base record for implementing an <see cref="IAddressValidationResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     An instance that implements <see cref="IApiResponse" /> which will be
///     returned from the underlying service api.
/// </typeparam>
/// <remarks>
///     Initializes a new instance of <see cref="AbstractAddressValidationResponse{T}" />.
/// </remarks>
/// <param name="response">An instance of <typeparamref name="TResponse" /> returned by the underlying api service.</param>
/// <param name="validationResult">
///     Current validation state (if any) of the response represented as an instance of
///     <see cref="IValidationResult" />.
/// </param>
#pragma warning disable CS9113 // Parameter is unread.
public abstract class AbstractAddressValidationResponse<TResponse>(TResponse response, IValidationResult? validationResult = null)
	: AbstractAddressValidationResponse(validationResult) where TResponse : IApiResponse;
#pragma warning restore CS9113 // Parameter is unread.
