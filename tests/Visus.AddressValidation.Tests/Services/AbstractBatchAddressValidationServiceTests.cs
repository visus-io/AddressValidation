namespace Visus.AddressValidation.Tests.Services;

using System.Diagnostics;
using Abstractions;
using Adapters;
using AddressValidation.Services;
using AddressValidation.Validation;
using Mappers;
using Models;
using NSubstitute;

internal sealed class AbstractBatchAddressValidationServiceTests : IDisposable
{
    private readonly DiagnosticsCapture _capture = new();

    private readonly IBatchApiRequestAdapter<TestAddressValidationRequest, TestApiResponse> _requestAdapter;

    private readonly IBatchApiResponseMapper<TestApiResponse> _responseMapper;

    private readonly TestBatchResponseValidator _responseValidator;

    private readonly TestBatchAddressValidationService _sut;

    public AbstractBatchAddressValidationServiceTests()
    {
        _requestAdapter = Substitute.For<IBatchApiRequestAdapter<TestAddressValidationRequest, TestApiResponse>>();
        _responseMapper = Substitute.For<IBatchApiResponseMapper<TestApiResponse>>();
        _responseValidator = new TestBatchResponseValidator();

        _sut = new TestBatchAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), _responseValidator);
    }

    [Test]
    public void Constructor_WhenBatchResponseValidatorIsNotAbstractBatchValidatorSubclass_ThrowsInvalidImplementationException()
    {
        IBatchValidator<TestApiResponse> notARealValidator = Substitute.For<IBatchValidator<TestApiResponse>>();

        Action act = () => _ = new TestBatchAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), notARealValidator);

        act.Should().ThrowExactly<InvalidImplementationException>();
    }

    [Test]
    public void Constructor_WhenBatchResponseValidatorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestBatchAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), null!);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRequestAdapterIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestBatchAddressValidationService(null!, _responseMapper, new TestRequestValidator(), _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRequestValidatorIsNotAbstractAddressValidationRequestValidatorSubclass_ThrowsInvalidImplementationException()
    {
        IValidator<TestAddressValidationRequest> notARealValidator = Substitute.For<IValidator<TestAddressValidationRequest>>();

        Action act = () => _ = new TestBatchAddressValidationService(_requestAdapter, _responseMapper, notARealValidator, _responseValidator);

        act.Should().ThrowExactly<InvalidImplementationException>();
    }

    [Test]
    public void Constructor_WhenRequestValidatorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestBatchAddressValidationService(_requestAdapter, _responseMapper, null!, _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenResponseMapperIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestBatchAddressValidationService(_requestAdapter, null!, new TestRequestValidator(), _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    public void Dispose()
    {
        _capture.Dispose();
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_RecordsBatchSizeTagOnActivity(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(),];
        TestApiResponse apiResponse = new();
        StubAdapter(requests, apiResponse);
        StubResponseValidator(false, false);
        StubResponseMapper(apiResponse);

        await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        Activity activity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate_many", StringComparison.Ordinal)).Subject;
        activity.GetTagItem("address_validation.batch_size").Should().Be(2);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_RecordsOnePerItemWarningAndSuggestionCountMeasurement(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(),];
        TestApiResponse apiResponse = new();
        StubAdapter(requests, apiResponse);
        StubResponseValidator(false, false);
        StubResponseMapper(apiResponse);

        await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        _capture.Measurements.Count(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_warning_count", StringComparison.Ordinal)).Should().Be(2);
        _capture.Measurements.Count(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_suggestion_count", StringComparison.Ordinal)).Should().Be(2);
    }

    [Test]
    public async Task ValidateManyAsync_WhenAdapterReturnsNull_ReturnsNullForEveryValidSlot(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(),];
        _requestAdapter.ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult<TestApiResponse?>(null));

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        results.Should().AllSatisfy(r => r.Should().BeNull());
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_WhenAdapterReturnsNullWithSomeInvalidRequests_RecordsPartialResultTag(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), new(),];
        _requestAdapter.ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult<TestApiResponse?>(null));

        await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        Activity activity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate_many", StringComparison.Ordinal)).Subject;
        activity.GetTagItem("address_validation.result").Should().Be("partial");
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_WhenAdapterThrows_PropagatesExceptionAndSetsActivityStatusError(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(),];
        InvalidOperationException thrown = new("adapter failed");
        _requestAdapter.ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>())
                       .Returns<Task<TestApiResponse?>>(_ => throw thrown);

        Func<Task> act = () => _sut.ValidateManyAsync(requests, cancellationToken);

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().ConfigureAwait(false);

        Activity activity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate_many", StringComparison.Ordinal)).Subject;
        activity.GetTagItem("address_validation.result").Should().Be("error");
        activity.Status.Should().Be(ActivityStatusCode.Error);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_WhenAllItemsSucceed_RecordsResultTagSuccess(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(),];
        TestApiResponse apiResponse = new();
        StubAdapter(requests, apiResponse);
        StubResponseValidator(false, false);
        StubResponseMapper(apiResponse);

        await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        Activity activity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate_many", StringComparison.Ordinal)).Subject;
        activity.GetTagItem("address_validation.result").Should().Be("success");
    }

    [Test]
    public async Task ValidateManyAsync_WhenAllRequestsFailValidation_ReturnsEmptyForEveryIndexAndSkipsAdapter(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [new(), new(),];

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        results.Should().AllSatisfy(r => r.Should().BeOfType<EmptyAddressValidationResponse>());
        await _requestAdapter.DidNotReceive().ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateManyAsync_WhenCountExceedsMaxBatchSize_ThrowsArgumentException(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(), ValidRequest(), ValidRequest(),];

        Func<Task> act = () => _sut.ValidateManyAsync(requests, cancellationToken);

        await act.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
        await _requestAdapter.DidNotReceive().ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateManyAsync_WhenMixedOutcomes_RecordsResultTagPartial(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), new(),];
        TestApiResponse apiResponse = new();
        StubAdapter([requests[0],], apiResponse);
        StubResponseValidator(false, false);
        StubResponseMapper(apiResponse);

        await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        Activity activity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate_many", StringComparison.Ordinal)).Subject;
        activity.GetTagItem("address_validation.result").Should().Be("partial");
    }

    [Test]
    public async Task ValidateManyAsync_WhenOneItemFailsResponseValidation_ReturnsEmptyOnlyForThatIndex_OthersStillMapped(CancellationToken cancellationToken)
    {
        List<TestAddressValidationRequest> requests = [ValidRequest(), ValidRequest(), ValidRequest(),];
        TestApiResponse apiResponse = new();
        StubAdapter(requests, apiResponse);
        _responseValidator.Stub = (_, _) =>
        [
            StubValidationResult(false),
            StubValidationResult(true),
            StubValidationResult(false),
        ];
        StubResponseMapper(apiResponse);

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        results[1].Should().BeOfType<EmptyAddressValidationResponse>();
        results[0].Should().NotBeOfType<EmptyAddressValidationResponse>();
        results[2].Should().NotBeOfType<EmptyAddressValidationResponse>();
    }

    [Test]
    public async Task ValidateManyAsync_WhenRequestsIsEmpty_ReturnsEmptyListAndDoesNotCallAdapter(CancellationToken cancellationToken)
    {
        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync([], cancellationToken).ConfigureAwait(false);

        results.Should().BeEmpty();
        await _requestAdapter.DidNotReceive().ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateManyAsync_WhenRequestsIsNull_ThrowsArgumentNullException()
    {
        Func<Task> act = () => _sut.ValidateManyAsync(null!, CancellationToken.None);

        await act.Should().ThrowExactlyAsync<ArgumentNullException>().ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateManyAsync_WhenSomeRequestsFailValidation_OnlySendsValidOnesAndPreservesOriginalOrder(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest invalid = new();
        TestAddressValidationRequest firstValid = ValidRequest();
        TestAddressValidationRequest secondValid = ValidRequest();
        List<TestAddressValidationRequest> requests = [firstValid, invalid, secondValid,];
        TestApiResponse apiResponse = new();
        List<TestAddressValidationRequest>? sentToAdapter = null;
        _requestAdapter.ExecuteAsync(Arg.Any<IReadOnlyList<TestAddressValidationRequest>>(), Arg.Any<CancellationToken>())
                       .Returns(callInfo =>
                        {
                            sentToAdapter = [.. callInfo.Arg<IReadOnlyList<TestAddressValidationRequest>>(),];
                            return Task.FromResult<TestApiResponse?>(apiResponse);
                        });
        StubResponseValidator(false, false);
        StubResponseMapper(apiResponse);

        IReadOnlyList<IAddressValidationResponse?> results = await _sut.ValidateManyAsync(requests, cancellationToken).ConfigureAwait(false);

        sentToAdapter.Should().Equal(firstValid, secondValid);
        results[1].Should().BeOfType<EmptyAddressValidationResponse>();
        results[0].Should().NotBeOfType<EmptyAddressValidationResponse>();
        results[2].Should().NotBeOfType<EmptyAddressValidationResponse>();
    }

    private static IAddressValidationResponse StubMappedResponse()
    {
        IAddressValidationResponse response = Substitute.For<IAddressValidationResponse>();
        response.Warnings.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        response.Suggestions.Returns([]);
        return response;
    }

    private static IValidationResult StubValidationResult(bool hasErrors)
    {
        IValidationResult result = Substitute.For<IValidationResult>();
        result.HasErrors.Returns(hasErrors);
        HashSet<ValidationState> errors = hasErrors ? [ValidationState.CreateError("invalid"),] : [];
        result.Errors.Returns(errors);
        result.Warnings.Returns(new HashSet<ValidationState>());
        return result;
    }

    private static TestAddressValidationRequest ValidRequest()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.US,
            CityOrTown = "Springfield",
            StateOrProvince = "IL",
            PostalCode = "62701",
        };
        request.AddressLines.Add("123 Main St");
        return request;
    }

    private void StubAdapter(List<TestAddressValidationRequest> expectedRequests, TestApiResponse apiResponse)
    {
        _requestAdapter.ExecuteAsync(Arg.Is<IReadOnlyList<TestAddressValidationRequest>>(r => r.Count == expectedRequests.Count), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult<TestApiResponse?>(apiResponse));
    }

    private void StubResponseMapper(TestApiResponse apiResponse)
    {
        _responseMapper.Map(apiResponse, Arg.Any<int>(), Arg.Any<IValidationResult>()).Returns(_ => StubMappedResponse());
    }

    private void StubResponseValidator(bool firstHasErrors, bool remainingHaveErrors)
    {
        _responseValidator.Stub = (_, count) =>
        {
            List<IValidationResult> results = [];
            for ( int i = 0; i < count; i++ )
            {
                results.Add(StubValidationResult(i == 0 ? firstHasErrors : remainingHaveErrors));
            }

            return results;
        };
    }

    internal sealed class TestAddressValidationRequest : AbstractAddressValidationRequest;

    internal sealed class TestApiResponse;

    private sealed class TestBatchAddressValidationService(
        IBatchApiRequestAdapter<TestAddressValidationRequest, TestApiResponse> requestAdapter,
        IBatchApiResponseMapper<TestApiResponse> responseMapper,
        IValidator<TestAddressValidationRequest> requestValidator,
        IBatchValidator<TestApiResponse> responseValidator)
        : AbstractBatchAddressValidationService<TestAddressValidationRequest, TestApiResponse>(requestAdapter, responseMapper, requestValidator, responseValidator)
    {
        protected override int MaxBatchSize => 3;
    }

    private sealed class TestBatchResponseValidator : AbstractBatchValidator<TestApiResponse>
    {
        internal Func<TestApiResponse, int, IReadOnlyList<IValidationResult>>? Stub { get; set; }

        protected override ValueTask<bool> PreValidateAsync(TestApiResponse instance, IReadOnlyList<int> requestIndexes, IReadOnlyList<ISet<ValidationState>> results, CancellationToken cancellationToken = default)
        {
            if ( Stub is null )
            {
                return ValueTask.FromResult(true);
            }

            IReadOnlyList<IValidationResult> stubbedResults = Stub(instance, requestIndexes.Count);
            for ( int i = 0; i < results.Count; i++ )
            {
                foreach ( ValidationState state in stubbedResults[i].Errors )
                {
                    results[i].Add(state);
                }

                foreach ( ValidationState state in stubbedResults[i].Warnings )
                {
                    results[i].Add(state);
                }
            }

            return ValueTask.FromResult(true);
        }
    }

    private sealed class TestRequestValidator : AbstractAddressValidationRequestValidator<TestAddressValidationRequest>
    {
        protected override string ProviderName => "Test";

        protected override FrozenSet<CountryCode> SupportedCountries =>
        [
            CountryCode.US,
        ];
    }
}
