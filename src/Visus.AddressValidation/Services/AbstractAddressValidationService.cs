namespace Visus.AddressValidation.Services;

using Mappers;
using Model;
using Validation;

/// <summary>
///     Abstract base class for implementing an <see cref="IAddressValidationService{TRequest}" />.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the validation request. Must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiRequest">
///     The type of the request sent to the underlying service API. Constructed from a
///     <typeparamref name="TRequest" /> by an <see cref="IApiRequestMapper{TRequest,TApiRequest}" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of the raw response returned by the underlying service API. Mapped to an
///     <see cref="IAddressValidationResponse" /> by an <see cref="IApiResponseMapper{TApiResponse}" />.
/// </typeparam>
public abstract class AbstractAddressValidationService<TRequest, TApiRequest, TApiResponse> : IAddressValidationService<TRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiRequest : class
    where TApiResponse : class
{
    private readonly IApiRequestMapper<TRequest, TApiRequest> _requestMapper;

    private readonly IValidator<TRequest> _requestValidator;

    private readonly IApiResponseMapper<TApiResponse> _responseMapper;

    private readonly IValidator<TApiResponse> _responseValidator;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractAddressValidationService{TRequest, TApiRequest, TApiResponse}" />.
    /// </summary>
    /// <param name="requestValidator">
    ///     An <see cref="IValidator{T}" /> used to validate <typeparamref name="TRequest" /> instances
    ///     before they are sent to the underlying API.
    /// </param>
    /// <param name="responseValidator">
    ///     An <see cref="IValidator{T}" /> used to validate <typeparamref name="TApiResponse" /> instances
    ///     returned by the underlying API.
    /// </param>
    /// <param name="responseMapper">
    ///     An <see cref="IApiResponseMapper{TApiResponse}" /> used to map <typeparamref name="TApiResponse" />
    ///     instances to an <see cref="IAddressValidationResponse" />.
    /// </param>
    /// <param name="requestMapper">
    ///     An <see cref="IApiRequestMapper{TRequest, TApiRequest}" /> used to map <typeparamref name="TRequest" />
    ///     instances to a <typeparamref name="TApiRequest" />.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="requestValidator" />, <paramref name="responseValidator" />,
    ///     <paramref name="responseMapper" />, or <paramref name="requestMapper" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidImplementationException">
    ///     Thrown when <paramref name="requestValidator" /> does not derive from
    ///     <see cref="AbstractAddressValidationRequestValidator{TRequest}" />.
    /// </exception>
    protected AbstractAddressValidationService(IValidator<TRequest> requestValidator,
                                               IValidator<TApiResponse> responseValidator,
                                               IApiResponseMapper<TApiResponse> responseMapper,
                                               IApiRequestMapper<TRequest, TApiRequest> requestMapper)
    {
        _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        _responseValidator = responseValidator ?? throw new ArgumentNullException(nameof(responseValidator));
        _responseMapper = responseMapper ?? throw new ArgumentNullException(nameof(responseMapper));
        _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));

        if ( !requestValidator.GetType().IsSubclassOf(typeof(AbstractAddressValidationRequestValidator<TRequest>)) )
        {
            throw new InvalidImplementationException($"{nameof(requestValidator)} must implement {nameof(AbstractAddressValidationRequestValidator<>)}");
        }
    }

    /// <inheritdoc />
    public Task<IAddressValidationResponse?> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ValidateInternalAsync(request, cancellationToken);
    }

    /// <summary>
    ///     Sends an asynchronous request to the underlying service API for address validation.
    /// </summary>
    /// <param name="request">The API request to send, represented as an instance of <typeparamref name="TApiRequest" />.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the raw API response as an instance
    ///     of <typeparamref name="TApiResponse" />, or <see langword="null" /> if the request failed.
    /// </returns>
    protected abstract Task<TApiResponse?> SendAsync(TApiRequest request, CancellationToken cancellationToken);

    private async Task<IAddressValidationResponse?> ValidateInternalAsync(TRequest request, CancellationToken cancellationToken)
    {
        IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        if ( requestValidationResult.HasErrors )
        {
            return new EmptyAddressValidationResponse(requestValidationResult);
        }

        TApiRequest apiRequest = _requestMapper.Map(request);
        TApiResponse? response = await SendAsync(apiRequest, cancellationToken).ConfigureAwait(false);
        
        if ( response is null )
        {
            return null;
        }

        IValidationResult responseValidationResult = await _responseValidator.ExecuteAsync(response, cancellationToken).ConfigureAwait(false);
        return responseValidationResult.HasErrors
                   ? new EmptyAddressValidationResponse(responseValidationResult)
                   : _responseMapper.Map(response, responseValidationResult);
    }
}
