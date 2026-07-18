namespace Visus.AddressValidation.Services;

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Adapters;
using Diagnostics;
using Mappers;
using Models;
using Resources;
using Validation;

/// <summary>
///     Abstract base class for implementing an <see cref="IBatchAddressValidationService{TRequest}" /> for a
///     provider whose API natively supports validating multiple addresses in a single call.
/// </summary>
/// <typeparam name="TRequest">
///     The type of the validation request. Must derive from <see cref="AbstractAddressValidationRequest" />.
/// </typeparam>
/// <typeparam name="TApiResponse">
///     The type of the raw response returned by the underlying service API. Mapped to
///     <see cref="IAddressValidationResponse" /> instances by an <see cref="IBatchApiResponseMapper{TResponse}" />.
/// </typeparam>
public abstract class AbstractBatchAddressValidationService<TRequest, TApiResponse> : IBatchAddressValidationService<TRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    private const string s_activityName = "address_validation.validate_many";

    private static readonly CompositeFormat s_batchExceedsMaximumSizeFormat = CompositeFormat.Parse(Resources.Validation_Batch_ExceedsMaximumSize);

    private const string s_resultError = "error";

    private const string s_resultInvalidRequest = "invalid_request";

    private const string s_resultInvalidResponse = "invalid_response";

    private const string s_resultNoResponse = "no_response";

    private const string s_resultPartial = "partial";

    private const string s_resultSuccess = "success";

    private const string s_sentinelBatchCountry = "batch";

    private const string s_tagBatchSize = "address_validation.batch_size";

    private const string s_tagCountry = "address_validation.country";

    private const string s_tagRequestType = "address_validation.request_type";

    private const string s_tagResult = "address_validation.result";

    private const string s_unknownCountry = "unknown";

    private readonly IBatchApiRequestAdapter<TRequest, TApiResponse> _batchRequestAdapter;

    private readonly IBatchApiResponseMapper<TApiResponse> _batchResponseMapper;

    private readonly IBatchValidator<TApiResponse> _batchResponseValidator;

    private readonly IValidator<TRequest> _requestValidator;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractBatchAddressValidationService{TRequest, TApiResponse}" />.
    /// </summary>
    /// <param name="batchRequestAdapter">
    ///     An <see cref="IBatchApiRequestAdapter{TRequest, TApiResponse}" /> used to forward a batch of
    ///     <typeparamref name="TRequest" /> instances to the underlying API and return the raw
    ///     <typeparamref name="TApiResponse" />.
    /// </param>
    /// <param name="batchResponseMapper">
    ///     An <see cref="IBatchApiResponseMapper{TResponse}" /> used to map individual items within a
    ///     <typeparamref name="TApiResponse" /> to an <see cref="IAddressValidationResponse" />.
    /// </param>
    /// <param name="requestValidator">
    ///     An <see cref="IValidator{T}" /> used to validate each <typeparamref name="TRequest" /> instance before it
    ///     is sent to the underlying API.
    /// </param>
    /// <param name="batchResponseValidator">
    ///     An <see cref="IBatchValidator{T}" /> used to validate the <typeparamref name="TApiResponse" /> returned by
    ///     the underlying API, producing one result per item sent.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="batchRequestAdapter" />, <paramref name="batchResponseMapper" />,
    ///     <paramref name="requestValidator" />, or <paramref name="batchResponseValidator" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidImplementationException">
    ///     Thrown when <paramref name="requestValidator" /> does not derive from
    ///     <see cref="AbstractAddressValidationRequestValidator{TRequest}" />, or when
    ///     <paramref name="batchResponseValidator" /> does not derive from <see cref="AbstractBatchValidator{TApiResponse}" />
    ///     .
    /// </exception>
    protected AbstractBatchAddressValidationService(IBatchApiRequestAdapter<TRequest, TApiResponse> batchRequestAdapter,
                                                    IBatchApiResponseMapper<TApiResponse> batchResponseMapper,
                                                    IValidator<TRequest> requestValidator,
                                                    IBatchValidator<TApiResponse> batchResponseValidator)
    {
        _batchRequestAdapter = batchRequestAdapter ?? throw new ArgumentNullException(nameof(batchRequestAdapter));
        _batchResponseMapper = batchResponseMapper ?? throw new ArgumentNullException(nameof(batchResponseMapper));
        _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        _batchResponseValidator = batchResponseValidator ?? throw new ArgumentNullException(nameof(batchResponseValidator));

        if ( requestValidator is not AbstractAddressValidationRequestValidator<TRequest> )
        {
            throw new InvalidImplementationException($"{nameof(requestValidator)} must derive from {nameof(AbstractAddressValidationRequestValidator<>)}");
        }

        if ( batchResponseValidator is not AbstractBatchValidator<TApiResponse> )
        {
            throw new InvalidImplementationException($"{nameof(batchResponseValidator)} must derive from {nameof(AbstractBatchValidator<>)}");
        }
    }

    /// <summary>
    ///     Gets the maximum number of requests this provider's API accepts in a single batch call.
    /// </summary>
    protected abstract int MaxBatchSize { get; }

    /// <inheritdoc />
    public Task<IReadOnlyList<IAddressValidationResponse?>> ValidateManyAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if ( requests.Count > MaxBatchSize )
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, s_batchExceedsMaximumSizeFormat, requests.Count, MaxBatchSize),
                nameof(requests));
        }

        return ValidateManyInternalAsync(requests, cancellationToken);
    }

    private static string CountryTag(TRequest request)
    {
        return request.Country?.ToString() ?? s_unknownCountry;
    }

    private static void RecordItemMetrics(string result, string country, IAddressValidationResponse? response)
    {
        if ( response is null )
        {
            return;
        }

        AddressValidationDiagnostics.ResponseWarningCount.Record(
            response.Warnings.Count,
            new KeyValuePair<string, object?>(s_tagRequestType, typeof(TRequest).Name),
            new KeyValuePair<string, object?>(s_tagResult, result),
            new KeyValuePair<string, object?>(s_tagCountry, country));

        AddressValidationDiagnostics.ResponseSuggestionCount.Record(
            response.Suggestions.Count,
            new KeyValuePair<string, object?>(s_tagRequestType, typeof(TRequest).Name),
            new KeyValuePair<string, object?>(s_tagResult, result),
            new KeyValuePair<string, object?>(s_tagCountry, country));
    }

    private async Task<bool> MapValidatedItemsAsync(TApiResponse apiResponse,
                                                    List<TRequest> validRequests,
                                                    List<int> validIndexes,
                                                    IAddressValidationResponse?[] finalResults,
                                                    CancellationToken cancellationToken)
    {
        IReadOnlyList<IValidationResult> perItemValidation =
            await _batchResponseValidator.ExecuteAsync(apiResponse, validIndexes, cancellationToken).ConfigureAwait(false);

        bool anyItemInvalid = false;
        for ( int j = 0; j < validIndexes.Count; j++ )
        {
            IValidationResult itemValidation = perItemValidation[j];
            IAddressValidationResponse itemResult = itemValidation.HasErrors
                                                        ? new EmptyAddressValidationResponse(itemValidation)
                                                        : _batchResponseMapper.Map(apiResponse, j, itemValidation);

            anyItemInvalid |= itemValidation.HasErrors;
            finalResults[validIndexes[j]] = itemResult;
            RecordItemMetrics(itemValidation.HasErrors ? s_resultInvalidResponse : s_resultSuccess, CountryTag(validRequests[j]), itemResult);
        }

        return anyItemInvalid;
    }

    private async Task<PartitionResult> PartitionByLocalValidationAsync(IReadOnlyList<TRequest> requests,
                                                                        IAddressValidationResponse?[] finalResults,
                                                                        CancellationToken cancellationToken)
    {
        List<int> validIndexes = [];
        List<TRequest> validRequests = [];

        for ( int i = 0; i < requests.Count; i++ )
        {
            IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(requests[i], cancellationToken).ConfigureAwait(false);
            if ( requestValidationResult.HasErrors )
            {
                finalResults[i] = new EmptyAddressValidationResponse(requestValidationResult);
                RecordItemMetrics(s_resultInvalidRequest, CountryTag(requests[i]), finalResults[i]);
                continue;
            }

            validIndexes.Add(i);
            validRequests.Add(requests[i]);
        }

        return new PartitionResult(validIndexes, validRequests);
    }

    private async Task<IReadOnlyList<IAddressValidationResponse?>> ValidateManyInternalAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken)
    {
        if ( requests.Count == 0 )
        {
            return [];
        }

        using Activity? activity = AddressValidationDiagnostics.ActivitySource.StartActivity(s_activityName);
        activity?.SetTag(s_tagRequestType, typeof(TRequest).Name);
        activity?.SetTag(s_tagBatchSize, requests.Count);
        activity?.SetTag(s_tagCountry, s_sentinelBatchCountry);

        long startTimestamp = Stopwatch.GetTimestamp();
        string result = s_resultSuccess;
        IAddressValidationResponse?[] finalResults = new IAddressValidationResponse?[requests.Count];

        try
        {
            PartitionResult partition = await PartitionByLocalValidationAsync(requests, finalResults, cancellationToken).ConfigureAwait(false);
            bool anyLocallyInvalid = partition.ValidRequests.Count != requests.Count;

            if ( partition.ValidRequests.Count == 0 )
            {
                result = s_resultInvalidRequest;
                return finalResults;
            }

            TApiResponse? apiResponse = await _batchRequestAdapter.ExecuteAsync(partition.ValidRequests, cancellationToken).ConfigureAwait(false);
            if ( apiResponse is null )
            {
                result = anyLocallyInvalid ? s_resultPartial : s_resultNoResponse;
                foreach ( int index in partition.ValidIndexes )
                {
                    finalResults[index] = null;
                }

                return finalResults;
            }

            bool anyItemInvalid = await MapValidatedItemsAsync(apiResponse, partition.ValidRequests, partition.ValidIndexes, finalResults, cancellationToken).ConfigureAwait(false);
            result = anyLocallyInvalid || anyItemInvalid ? s_resultPartial : s_resultSuccess;
            return finalResults;
        }
        catch ( Exception ex )
        {
            result = s_resultError;
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
        finally
        {
            activity?.SetTag(s_tagResult, result);
            AddressValidationDiagnostics.ValidationDuration.Record(
                Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                new KeyValuePair<string, object?>(s_tagRequestType, typeof(TRequest).Name),
                new KeyValuePair<string, object?>(s_tagResult, result),
                new KeyValuePair<string, object?>(s_tagCountry, s_sentinelBatchCountry));
        }
    }

    private readonly record struct PartitionResult(List<int> ValidIndexes, List<TRequest> ValidRequests);
}
