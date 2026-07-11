namespace Visus.AddressValidation.Services;

using System.Diagnostics;
using Adapters;
using Diagnostics;
using Mappers;
using Models;
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

    private static void RecordMetrics(long startTimestamp, string result, string country, IAddressValidationResponse? response)
    {
        AddressValidationDiagnostics.ValidationDuration.Record(
            Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
            new KeyValuePair<string, object?>("address_validation.request_type", typeof(TRequest).Name),
            new KeyValuePair<string, object?>("address_validation.result", result),
            new KeyValuePair<string, object?>("address_validation.country", country));

        if ( response is null )
        {
            return;
        }

        AddressValidationDiagnostics.ResponseWarningCount.Record(
            response.Warnings.Count,
            new KeyValuePair<string, object?>("address_validation.request_type", typeof(TRequest).Name),
            new KeyValuePair<string, object?>("address_validation.result", result),
            new KeyValuePair<string, object?>("address_validation.country", country));

        AddressValidationDiagnostics.ResponseSuggestionCount.Record(
            response.Suggestions.Count,
            new KeyValuePair<string, object?>("address_validation.request_type", typeof(TRequest).Name),
            new KeyValuePair<string, object?>("address_validation.result", result),
            new KeyValuePair<string, object?>("address_validation.country", country));
    }

    private async Task<IAddressValidationResponse?> ValidateInternalAsync(TRequest request, CancellationToken cancellationToken)
    {
        using Activity? activity = AddressValidationDiagnostics.ActivitySource.StartActivity("address_validation.validate");
        string country = request.Country?.ToString() ?? "unknown";
        activity?.SetTag("address_validation.request_type", typeof(TRequest).Name);
        activity?.SetTag("address_validation.country", country);

        long startTimestamp = Stopwatch.GetTimestamp();
        string result = "success";
        IAddressValidationResponse? response = null;

        try
        {
            IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
            if ( requestValidationResult.HasErrors )
            {
                result = "invalid_request";
                response = new EmptyAddressValidationResponse(requestValidationResult);
                return response;
            }

            TApiResponse? apiResponse = await _requestAdapter.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
            if ( apiResponse is null )
            {
                result = "no_response";
                return null;
            }

            IValidationResult responseValidationResult = await _responseValidator.ExecuteAsync(apiResponse, cancellationToken).ConfigureAwait(false);
            if ( responseValidationResult.HasErrors )
            {
                result = "invalid_response";
                response = new EmptyAddressValidationResponse(responseValidationResult);
                return response;
            }

            response = _responseMapper.Map(apiResponse, responseValidationResult);
            return response;
        }
        catch ( Exception ex )
        {
            result = "error";
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
        finally
        {
            activity?.SetTag("address_validation.result", result);
            RecordMetrics(startTimestamp, result, country, response);
        }
    }
}
