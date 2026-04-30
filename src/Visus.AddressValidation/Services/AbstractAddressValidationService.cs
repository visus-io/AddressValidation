namespace Visus.AddressValidation.Services;

using Adapters;
using Mappers;
using Model;
using Validation;

/// <summary>
///     Abstract base class for implementing an <see cref="IAddressValidationService{TRequest}" />.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the validation request. Must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of the raw response returned by the underlying service API. Mapped to an
///     <see cref="IAddressValidationResponse" /> by an <see cref="IApiResponseMapper{TApiResponse}" />.
/// </typeparam>
public abstract class AbstractAddressValidationService<TRequest, TApiResponse> : IAddressValidationService<TRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    private readonly IApiRequestAdapter<TRequest, TApiResponse> _requestAdapter;

    private readonly IValidator<TRequest> _requestValidator;

    private readonly IApiResponseMapper<TApiResponse> _responseMapper;

    private readonly IValidator<TApiResponse> _responseValidator;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractAddressValidationService{TRequest, TApiResponse}" />.
    /// </summary>
    /// <param name="requestAdapter">
    ///     An <see cref="IApiRequestAdapter{TRequest, TApiResponse}" /> used to forward
    ///     <typeparamref name="TRequest" /> instances to the underlying API and return the raw
    ///     <typeparamref name="TApiResponse" />.
    /// </param>
    /// <param name="responseMapper">
    ///     An <see cref="IApiResponseMapper{TApiResponse}" /> used to map <typeparamref name="TApiResponse" />
    ///     instances to an <see cref="IAddressValidationResponse" />.
    /// </param>
    /// <param name="requestValidator">
    ///     An <see cref="IValidator{T}" /> used to validate <typeparamref name="TRequest" /> instances
    ///     before they are sent to the underlying API.
    /// </param>
    /// <param name="responseValidator">
    ///     An <see cref="IValidator{T}" /> used to validate <typeparamref name="TApiResponse" /> instances
    ///     returned by the underlying API.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="requestAdapter" />, <paramref name="responseMapper" />,
    ///     <paramref name="requestValidator" />, or <paramref name="responseValidator" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidImplementationException">
    ///     Thrown when <paramref name="requestValidator" /> does not derive from
    ///     <see cref="AbstractAddressValidationRequestValidator{TRequest}" />.
    /// </exception>
    protected AbstractAddressValidationService(IApiRequestAdapter<TRequest, TApiResponse> requestAdapter,
                                               IApiResponseMapper<TApiResponse> responseMapper,
                                               IValidator<TRequest> requestValidator,
                                               IValidator<TApiResponse> responseValidator)
    {
        _requestAdapter = requestAdapter ?? throw new ArgumentNullException(nameof(requestAdapter));
        _responseMapper = responseMapper ?? throw new ArgumentNullException(nameof(responseMapper));
        _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        _responseValidator = responseValidator ?? throw new ArgumentNullException(nameof(responseValidator));

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

    private async Task<IAddressValidationResponse?> ValidateInternalAsync(TRequest request, CancellationToken cancellationToken)
    {
        IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        if ( requestValidationResult.HasErrors )
        {
            return new EmptyAddressValidationResponse(requestValidationResult);
        }

        TApiResponse? response = await _requestAdapter.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
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
