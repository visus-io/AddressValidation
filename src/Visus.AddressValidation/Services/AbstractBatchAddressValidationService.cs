namespace Visus.AddressValidation.Services;

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Abstractions;
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
///     The type of API response the provider returns. An <see cref="IBatchApiResponseMapper{TResponse}" />
///     maps it to <see cref="IAddressValidationResponse" /> instances.
/// </typeparam>
public abstract class AbstractBatchAddressValidationService<TRequest, TApiResponse> : IBatchAddressValidationService<TRequest>
    where TRequest : AbstractAddressValidationRequest
    where TApiResponse : class
{
    private const string s_activityName = "address_validation.validate_many";

    private static readonly CompositeFormat s_batchExceedsMaximumSizeFormat = CompositeFormat.Parse(Resources.Validation_Batch_ExceedsMaximumSize);

    private const string s_resultPartial = "partial";

    private const string s_sentinelBatchCountry = "batch";

    private const string s_tagBatchSize = "address_validation.batch_size";

    private readonly IBatchApiRequestAdapter<TRequest, TApiResponse> _batchRequestAdapter;

    private readonly IBatchApiResponseMapper<TApiResponse> _batchResponseMapper;

    private readonly IBatchValidator<TApiResponse> _batchResponseValidator;

    private readonly IValidator<TRequest> _requestValidator;

    /// <summary>
    ///     Initializes a new instance of <see cref="AbstractBatchAddressValidationService{TRequest, TApiResponse}" />.
    /// </summary>
    /// <param name="batchRequestAdapter">
    ///     An <see cref="IBatchApiRequestAdapter{TRequest, TApiResponse}" /> that sends a batch of
    ///     <typeparamref name="TRequest" /> instances to the provider and returns the
    ///     <typeparamref name="TApiResponse" />.
    /// </param>
    /// <param name="batchResponseMapper">
    ///     An <see cref="IBatchApiResponseMapper{TResponse}" /> that maps individual items within a
    ///     <typeparamref name="TApiResponse" /> to an <see cref="IAddressValidationResponse" />.
    /// </param>
    /// <param name="requestValidator">
    ///     An <see cref="IValidator{T}" /> that validates each <typeparamref name="TRequest" /> instance before the
    ///     service sends it to the provider.
    /// </param>
    /// <param name="batchResponseValidator">
    ///     An <see cref="IBatchValidator{T}" /> that validates the <typeparamref name="TApiResponse" /> the provider
    ///     returns. It produces one result per item sent.
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

    private static string ComputeBatchCountryTag(IReadOnlyList<TRequest> requests)
    {
        CountryCode? firstCountry = requests[0].Country;

        for ( int i = 1; i < requests.Count; i++ )
        {
            if ( requests[i].Country != firstCountry )
            {
                return s_sentinelBatchCountry;
            }
        }

        return AddressValidationServiceDiagnostics.CountryTag(requests[0]);
    }

    private static void RecordItemMetrics(string result, string country, IAddressValidationResponse? response)
    {
        AddressValidationServiceDiagnostics.RecordResponseCounts(typeof(TRequest).Name, result, country, response);
    }

    private async Task<bool> MapValidatedItemsAsync(TApiResponse apiResponse,
                                                    IReadOnlyList<PartitionedRequest> validPartition,
                                                    IAddressValidationResponse?[] finalResults,
                                                    CancellationToken cancellationToken)
    {
        IReadOnlyList<int> validIndexes = [.. validPartition.Select(static p => p.OriginalIndex),];
        IReadOnlyList<IValidationResult> perItemValidation =
            await _batchResponseValidator.ExecuteAsync(apiResponse, validIndexes, cancellationToken).ConfigureAwait(false);

        if ( perItemValidation.Count != validPartition.Count )
        {
            throw new InvalidImplementationException(
                $"{nameof(IBatchValidator<>)}.{nameof(IBatchValidator<>.ExecuteAsync)} must return exactly one " +
                $"{nameof(IValidationResult)} per sent request ({validPartition.Count}), but returned {perItemValidation.Count}.");
        }

        bool anyItemInvalid = false;
        for ( int j = 0; j < validPartition.Count; j++ )
        {
            PartitionedRequest partitioned = validPartition[j];
            IValidationResult itemValidation = perItemValidation[j];
            IAddressValidationResponse itemResult = itemValidation.HasErrors
                                                        ? new EmptyAddressValidationResponse(itemValidation)
                                                        : _batchResponseMapper.Map(apiResponse, j, itemValidation);

            anyItemInvalid |= itemValidation.HasErrors;
            finalResults[partitioned.OriginalIndex] = itemResult;
            RecordItemMetrics(
                itemValidation.HasErrors ? AddressValidationServiceDiagnostics.s_resultInvalidResponse : AddressValidationServiceDiagnostics.s_resultSuccess,
                AddressValidationServiceDiagnostics.CountryTag(partitioned.Request),
                itemResult);
        }

        return anyItemInvalid;
    }

    private async Task<List<PartitionedRequest>> PartitionByLocalValidationAsync(IReadOnlyList<TRequest> requests,
                                                                                 IAddressValidationResponse?[] finalResults,
                                                                                 CancellationToken cancellationToken)
    {
        List<PartitionedRequest> validPartition = [];

        for ( int i = 0; i < requests.Count; i++ )
        {
            IValidationResult requestValidationResult = await _requestValidator.ExecuteAsync(requests[i], cancellationToken).ConfigureAwait(false);
            if ( requestValidationResult.HasErrors )
            {
                finalResults[i] = new EmptyAddressValidationResponse(requestValidationResult);
                RecordItemMetrics(AddressValidationServiceDiagnostics.s_resultInvalidRequest, AddressValidationServiceDiagnostics.CountryTag(requests[i]), finalResults[i]);
                continue;
            }

            validPartition.Add(new PartitionedRequest(i, requests[i]));
        }

        return validPartition;
    }

    private async Task<IReadOnlyList<IAddressValidationResponse?>> ValidateManyInternalAsync(IReadOnlyList<TRequest> requests, CancellationToken cancellationToken)
    {
        if ( requests.Count == 0 )
        {
            return [];
        }

        using Activity? activity = AddressValidationDiagnostics.ActivitySource.StartActivity(s_activityName);
        string countryTag = ComputeBatchCountryTag(requests);
        activity?.SetTag(AddressValidationServiceDiagnostics.s_tagRequestType, typeof(TRequest).Name);
        activity?.SetTag(s_tagBatchSize, requests.Count);
        activity?.SetTag(AddressValidationServiceDiagnostics.s_tagCountry, countryTag);

        long startTimestamp = Stopwatch.GetTimestamp();
        string result = AddressValidationServiceDiagnostics.s_resultSuccess;
        IAddressValidationResponse?[] finalResults = new IAddressValidationResponse?[requests.Count];

        try
        {
            List<PartitionedRequest> validPartition = await PartitionByLocalValidationAsync(requests, finalResults, cancellationToken).ConfigureAwait(false);
            bool anyLocallyInvalid = validPartition.Count != requests.Count;

            if ( validPartition.Count == 0 )
            {
                result = AddressValidationServiceDiagnostics.s_resultInvalidRequest;
                return finalResults;
            }

            IReadOnlyList<TRequest> validRequests = [.. validPartition.Select(static p => p.Request),];
            TApiResponse? apiResponse = await _batchRequestAdapter.ExecuteAsync(validRequests, cancellationToken).ConfigureAwait(false);
            if ( apiResponse is null )
            {
                result = anyLocallyInvalid ? s_resultPartial : AddressValidationServiceDiagnostics.s_resultNoResponse;
                return finalResults;
            }

            bool anyItemInvalid = await MapValidatedItemsAsync(apiResponse, validPartition, finalResults, cancellationToken).ConfigureAwait(false);
            result = anyLocallyInvalid || anyItemInvalid ? s_resultPartial : AddressValidationServiceDiagnostics.s_resultSuccess;
            return finalResults;
        }
        catch ( Exception ex )
        {
            result = AddressValidationServiceDiagnostics.s_resultError;
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
        finally
        {
            activity?.SetTag(AddressValidationServiceDiagnostics.s_tagResult, result);
            AddressValidationDiagnostics.ValidationDuration.Record(
                Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds,
                new KeyValuePair<string, object?>(AddressValidationServiceDiagnostics.s_tagRequestType, typeof(TRequest).Name),
                new KeyValuePair<string, object?>(AddressValidationServiceDiagnostics.s_tagResult, result),
                new KeyValuePair<string, object?>(AddressValidationServiceDiagnostics.s_tagCountry, countryTag));
        }
    }

    private readonly record struct PartitionedRequest(int OriginalIndex, TRequest Request);
}
