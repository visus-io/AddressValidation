namespace Visus.AddressValidation.Services;

using System.Runtime.CompilerServices;
using Http;
using Model;
using Validation;

/// <summary>
///     Base class for implementing an <see cref="IAddressValidationService{TRequest}" />.
/// </summary>
/// <typeparam name="TRequest">
///     An instance that implements <see cref="AbstractAddressValidationRequest" /> which will be
///     used for validation.
/// </typeparam>
/// <typeparam name="TResponse">
///     An instance that implements <see cref="AbstractApiResponse" /> which will be
///     returned from the underlying service api.
/// </typeparam>
public abstract class AbstractAddressValidationService<TRequest, TResponse> : IAddressValidationService<TRequest>
	where TRequest : AbstractAddressValidationRequest
	where TResponse : AbstractApiResponse
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
	///     is <see langword="null" />.
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	///     The instance of <typeparamref name="TResponse" /> may be <see langword="null" /> if the request failed.
	/// </remarks>
	/// <returns>Response from the underlying service api represented by an instance of <typeparamref name="TResponse" />.</returns>
	protected abstract ValueTask<TResponse?> SendAsync(TRequest request, CancellationToken cancellationToken);

	private async ValueTask<IAddressValidationResponse?> ValidateInternalAsync(TRequest request, CancellationToken cancellationToken)
	{
		IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
		if ( requestValidationResult.HasErrors )
		{
			return new EmptyAddressValidationResponse(requestValidationResult);
		}

		TResponse? response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
		if ( response is null )
		{
			return null;
		}

		IValidationResult responseValidationResult = await _responseValidator.ExecuteAsync(response, cancellationToken).ConfigureAwait(false);
		return responseValidationResult.HasErrors
				   ? new EmptyAddressValidationResponse(responseValidationResult)
				   : response.ToAddressValidationResponse(responseValidationResult);
	}
}
