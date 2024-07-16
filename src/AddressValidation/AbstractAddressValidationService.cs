namespace Visus.AddressValidation;

using Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Http.Abstractions;
using Model;

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationService{TRequest}" />.
/// </summary>
/// <typeparam name="TRequest">The request that will be used for validation</typeparam>
/// <typeparam name="TResponse">The response that will be returned from the underlying service api.</typeparam>
public abstract class AbstractAddressValidationService<TRequest, TResponse> : IAddressValidationService<TRequest>
	where TRequest : AbstractAddressValidationRequest
	where TResponse : IApiAddressValidationResponse
{
	private readonly IValidator<TRequest> _requestValidator;

	private readonly IValidator<TResponse> _responseValidator;

	/// <summary>
	///     Initializes a new instance of <see cref="AbstractAddressValidationService{TRequest, TResponse}" />.
	/// </summary>
	/// <param name="requestValidator">
	///     An instance of <see cref="IValidator{T}" /> for validating
	///     <typeparamref name="TRequest" /> objects.
	/// </param>
	/// <param name="responseValidator">
	///     An instance of <see cref="IValidator{T}" /> for validating
	///     <typeparamref name="TResponse" /> objects.
	/// </param>
	/// <exception cref="InvalidImplementationException">
	///     <paramref name="requestValidator" /> does not implement
	///     <see cref="AbstractAddressValidationRequestValidator{TRequest}" />.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="requestValidator" /> or <paramref name="responseValidator" />
	///     is <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).
	/// </exception>
	protected AbstractAddressValidationService(IValidator<TRequest> requestValidator,
											   IValidator<TResponse> responseValidator)
	{
		_requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
		_responseValidator = responseValidator ?? throw new ArgumentNullException(nameof(responseValidator));

		if ( !requestValidator.GetType().IsSubclassOf(typeof(AbstractAddressValidationRequestValidator<TRequest>)) )
		{
			throw new InvalidImplementationException($"{nameof(requestValidator)} must implement {nameof(AbstractAddressValidationRequestValidator<TRequest>)}");
		}
	}

	/// <inheritdoc />
	public ValueTask<IAddressValidationResponse?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);
		return ValidateInternalAsync(request, cancellationToken);
	}

	/// <summary>
	///     Send an asynchronous request to the underlying service api for validation.
	/// </summary>
	/// <param name="request">Request to be validated represented as an instance of <typeparamref name="TRequest" />.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
	/// <remarks>
	///     The instance of <typeparamref name="TResponse" /> may be <see langword="null" /> (<see langword="Nothing" />
	///     in Visual Basic) if the request failed.
	/// </remarks>
	/// <returns>Response from the underlying service api represented by an instance of <typeparamref name="TResponse" />.</returns>
	protected abstract ValueTask<TResponse?> SendAsync(TRequest request, CancellationToken cancellationToken);

	private async ValueTask<IAddressValidationResponse?> ValidateInternalAsync(TRequest request, CancellationToken cancellationToken)
	{
		ValidationResult? requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);
		if ( requestValidationResult is not null &&
			 !requestValidationResult.IsValid &&
			 requestValidationResult.Errors.Exists(e => e.Severity == Severity.Error) )
		{
			return new EmptyAddressValidationResponse(requestValidationResult);
		}

		TResponse? response = await SendAsync(request, cancellationToken);
		if ( response is null )
		{
			return null;
		}

		ValidationResult? responseValidationResult = await _responseValidator.ValidateAsync(response, cancellationToken);
		if ( responseValidationResult is not null &&
			 !responseValidationResult.IsValid &&
			 responseValidationResult.Errors.Exists(e => e.Severity == Severity.Error) )
		{
			return new EmptyAddressValidationResponse(requestValidationResult);
		}

		return response.ToAddressValidationResponse(responseValidationResult);
	}
}
