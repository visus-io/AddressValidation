namespace Visus.AddressValidation;

using System.Collections.ObjectModel;
using Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Http.Abstractions;

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationResponse" />.
/// </summary>
public abstract class AbstractAddressValidationResponse : IAddressValidationResponse
{
	/// <summary>
	///     Initializes a new instance of the <see cref="AbstractAddressValidationResponse" /> class.
	/// </summary>
	/// <param name="validationResult">
	///     Current validation state of the response represented as an instance of
	///     <see cref="ValidationResult" />.
	/// </param>
	protected AbstractAddressValidationResponse(ValidationResult? validationResult)
	{
		if ( validationResult is null )
		{
			return;
		}

		Errors = validationResult.Errors
								 .Where(w => w.Severity == Severity.Error)
								 .Select(s => !string.IsNullOrWhiteSpace(s.ErrorCode) ? $"{s.ErrorCode}: {s.ErrorMessage}" : $"{s.ErrorMessage}")
								 .Distinct()
								 .ToList()
								 .AsReadOnly();

		Warnings = validationResult.Errors
								   .Where(w => w.Severity == Severity.Warning)
								   .Select(s => !string.IsNullOrWhiteSpace(s.ErrorCode) ? $"{s.ErrorCode}: {s.ErrorMessage}" : $"{s.ErrorMessage}")
								   .Distinct()
								   .ToList()
								   .AsReadOnly();
	}

	/// <inheritdoc />
	public IReadOnlyCollection<string> Errors { get; } = ReadOnlyCollection<string>.Empty;

	/// <inheritdoc />
	public IReadOnlyCollection<string> Warnings { get; } = ReadOnlyCollection<string>.Empty;

	/// <inheritdoc />
	public IReadOnlyCollection<string> AddressLines { get; protected init; } = ReadOnlyCollection<string>.Empty;

	/// <inheritdoc />
	public string? CityOrTown { get; protected init; }

	/// <inheritdoc />
	public CountryCode Country { get; protected init; }

	/// <inheritdoc />
	public string? PostalCode { get; protected init; }

	/// <inheritdoc />
	public string? StateOrProvince { get; protected init; }

	/// <inheritdoc />
	public IReadOnlyCollection<IAddressValidationResponse> Suggestions { get; protected set; } = ReadOnlyCollection<IAddressValidationResponse>.Empty;
}

/// <inheritdoc />
public abstract class AbstractAddressValidationResponse<TResponse> : AbstractAddressValidationResponse
	where TResponse : IApiAddressValidationResponse
{
	/// <summary>
	///     Initializes a new instance of <see cref="AbstractAddressValidationResponse{T}" />.
	/// </summary>
	/// <param name="response">An instance of <typeparamref name="TResponse" /> returned by the underlying api service.</param>
	/// <param name="validationResult">
	///     Current validation state (if any) of the response represented as an instance of
	///     <see cref="ValidationResult" />.
	/// </param>
	protected AbstractAddressValidationResponse(TResponse response, ValidationResult? validationResult)
		: base(validationResult)
	{
	}
}
