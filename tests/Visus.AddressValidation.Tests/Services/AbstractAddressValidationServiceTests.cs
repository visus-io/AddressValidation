namespace Visus.AddressValidation.Tests.Services;

using System.Diagnostics;
using Abstractions;
using Adapters;
using AddressValidation.Services;
using AddressValidation.Validation;
using Mappers;
using Models;
using NSubstitute;

internal sealed class AbstractAddressValidationServiceTests : IDisposable
{
    private readonly DiagnosticsCapture _capture = new();

    private readonly IApiRequestAdapter<TestAddressValidationRequest, TestApiResponse> _requestAdapter;

    private readonly IApiResponseMapper<TestApiResponse> _responseMapper;

    private readonly TestResponseValidator _responseValidator;

    private readonly TestAddressValidationService _sut;

    public AbstractAddressValidationServiceTests()
    {
        _requestAdapter = Substitute.For<IApiRequestAdapter<TestAddressValidationRequest, TestApiResponse>>();
        _responseMapper = Substitute.For<IApiResponseMapper<TestApiResponse>>();
        _responseValidator = new TestResponseValidator();

        _sut = new TestAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), _responseValidator);
    }

    [Test]
    public void Constructor_WhenRequestAdapterIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(null!, _responseMapper, new TestRequestValidator(), _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRequestValidatorIsNotAbstractAddressValidationRequestValidatorSubclass_ThrowsInvalidImplementationException()
    {
        IValidator<TestAddressValidationRequest> notARealValidator = Substitute.For<IValidator<TestAddressValidationRequest>>();

        Action act = () => _ = new TestAddressValidationService(_requestAdapter, _responseMapper, notARealValidator, _responseValidator);

        act.Should().ThrowExactly<InvalidImplementationException>();
    }

    [Test]
    public void Constructor_WhenRequestValidatorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(_requestAdapter, _responseMapper, null!, _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenResponseMapperIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(_requestAdapter, null!, new TestRequestValidator(), _responseValidator);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenResponseValidatorIsNotAbstractValidatorSubclass_ThrowsInvalidImplementationException()
    {
        IValidator<TestApiResponse> notARealValidator = Substitute.For<IValidator<TestApiResponse>>();

        Action act = () => _ = new TestAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), notARealValidator);

        act.Should().ThrowExactly<InvalidImplementationException>();
    }

    [Test]
    public void Constructor_WhenResponseValidatorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(_requestAdapter, _responseMapper, new TestRequestValidator(), null!);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    public void Dispose()
    {
        _capture.Dispose();
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenAdapterReturnsNull_RecordsResultTagNoResponse(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(null));

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.result").Should().Be("no_response");
    }

    [Test]
    public async Task ValidateAsync_WhenAdapterReturnsNull_ReturnsNullAndDoesNotCallResponseValidator(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(null));

        IAddressValidationResponse? result = await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        result.Should().BeNull();
        await _responseValidator.Inner.DidNotReceive().ExecuteAsync(Arg.Any<TestApiResponse>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenAdapterReturnsNull_DoesNotRecordResponseWarningOrSuggestionCount(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(null));

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        _capture.Measurements.Should().NotContain(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_warning_count", StringComparison.Ordinal));
        _capture.Measurements.Should().NotContain(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_suggestion_count", StringComparison.Ordinal));
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenAdapterThrows_RecordsResultTagErrorSetsActivityStatusErrorAndPropagatesException(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        InvalidOperationException thrown = new("adapter failed");
        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns<Task<TestApiResponse?>>(_ => throw thrown);

        Func<Task> act = () => _sut.ValidateAsync(request, cancellationToken);

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.result").Should().Be("error");
        validateActivity.Status.Should().Be(ActivityStatusCode.Error);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenAdapterThrows_DoesNotRecordResponseWarningOrSuggestionCount(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        InvalidOperationException thrown = new("adapter failed");
        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns<Task<TestApiResponse?>>(_ => throw thrown);

        Func<Task> act = () => _sut.ValidateAsync(request, cancellationToken);

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().ConfigureAwait(false);

        _capture.Measurements.Should().NotContain(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_warning_count", StringComparison.Ordinal));
        _capture.Measurements.Should().NotContain(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_suggestion_count", StringComparison.Ordinal));
    }

    [Test]
    public async Task ValidateAsync_WhenAllStepsSucceed_ReturnsMapperResult(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        TestApiResponse apiResponse = new();
        IAddressValidationResponse mapped = StubMappedResponse();

        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(apiResponse));
        StubResponseValidator(apiResponse, false);
        _responseMapper.Map(apiResponse, Arg.Any<IValidationResult>()).Returns(mapped);

        IAddressValidationResponse? result = await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        result.Should().BeSameAs(mapped);
    }

    [Test]
    public async Task ValidateAsync_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        Func<Task> act = () => _sut.ValidateAsync(null!, CancellationToken.None);

        await act.Should().ThrowExactlyAsync<ArgumentNullException>().ConfigureAwait(false);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenRequestHasNoCountry_RecordsCountryTagAsUnknown(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = new();

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.country").Should().Be("unknown");
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenRequestValidationFails_RecordsResponseSuggestionCountAsZero(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = new();

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        DiagnosticsCapture.Measurement suggestionMeasurement = _capture.Measurements
                                                                       .Should()
                                                                       .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_suggestion_count", StringComparison.Ordinal))
                                                                       .Subject;
        suggestionMeasurement.Value.Should().Be(0);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenRequestValidationFails_RecordsResultTagInvalidRequest(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = new();

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.result").Should().Be("invalid_request");
    }

    [Test]
    public async Task ValidateAsync_WhenRequestValidationFails_ReturnsEmptyResponseAndDoesNotCallAdapter(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = new();

        IAddressValidationResponse? result = await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        result.Should().BeOfType<EmptyAddressValidationResponse>();
        await _requestAdapter.DidNotReceive().ExecuteAsync(Arg.Any<TestAddressValidationRequest>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenResponseValidationFails_RecordsResultTagInvalidResponse(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        TestApiResponse apiResponse = new();

        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(apiResponse));
        StubResponseValidator(apiResponse, true);

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.result").Should().Be("invalid_response");
    }

    [Test]
    public async Task ValidateAsync_WhenResponseValidationFails_ReturnsEmptyResponseAndDoesNotCallMapper(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        TestApiResponse apiResponse = new();

        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(apiResponse));
        StubResponseValidator(apiResponse, true);

        IAddressValidationResponse? result = await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        result.Should().BeOfType<EmptyAddressValidationResponse>();
        _responseMapper.DidNotReceive().Map(Arg.Any<TestApiResponse>(), Arg.Any<IValidationResult>());
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenSuccessful_RecordsActivityAndHistogramWithSuccessResult(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        TestApiResponse apiResponse = new();
        IAddressValidationResponse mapped = StubMappedResponse();

        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(apiResponse));
        StubResponseValidator(apiResponse, false);
        _responseMapper.Map(apiResponse, Arg.Any<IValidationResult>()).Returns(mapped);

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        Activity validateActivity = _capture.Activities.Should().ContainSingle(a => string.Equals(a.OperationName, "address_validation.validate", StringComparison.Ordinal)).Subject;
        validateActivity.GetTagItem("address_validation.request_type").Should().Be(nameof(TestAddressValidationRequest));
        validateActivity.GetTagItem("address_validation.result").Should().Be("success");
        validateActivity.GetTagItem("address_validation.country").Should().Be(nameof(CountryCode.US));
        validateActivity.Status.Should().Be(ActivityStatusCode.Unset);

        DiagnosticsCapture.Measurement measurement = _capture.Measurements
                                                             .Should()
                                                             .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.duration", StringComparison.Ordinal))
                                                             .Subject;
        measurement.Value.Should().BeGreaterThanOrEqualTo(0);
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.request_type", nameof(TestAddressValidationRequest)));
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.result", "success"));
        measurement.Tags.Should().Contain(new KeyValuePair<string, object?>("address_validation.country", nameof(CountryCode.US)));
    }

    [Test]
    [NotInParallel]
    public async Task ValidateAsync_WhenSuccessfulWithWarningsAndSuggestions_RecordsResponseWarningAndSuggestionCounts(CancellationToken cancellationToken)
    {
        TestAddressValidationRequest request = ValidRequest();
        TestApiResponse apiResponse = new();
        IAddressValidationResponse suggestion = StubMappedResponse();
        IAddressValidationResponse mapped = StubMappedResponse();
        mapped.Warnings.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "address was interpolated",
        });
        mapped.Suggestions.Returns([suggestion]);

        _requestAdapter.ExecuteAsync(request, Arg.Any<CancellationToken>()).Returns(Task.FromResult<TestApiResponse?>(apiResponse));
        StubResponseValidator(apiResponse, false);
        _responseMapper.Map(apiResponse, Arg.Any<IValidationResult>()).Returns(mapped);

        await _sut.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        DiagnosticsCapture.Measurement warningMeasurement = _capture.Measurements
                                                                    .Should()
                                                                    .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_warning_count", StringComparison.Ordinal))
                                                                    .Subject;
        warningMeasurement.Value.Should().Be(1);

        DiagnosticsCapture.Measurement suggestionMeasurement = _capture.Measurements
                                                                       .Should()
                                                                       .ContainSingle(m => string.Equals(m.InstrumentName, "visus.address_validation.validate.response_suggestion_count", StringComparison.Ordinal))
                                                                       .Subject;
        suggestionMeasurement.Value.Should().Be(1);
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
        result.Errors.Returns(hasErrors
                                  ? new HashSet<ValidationState>
                                  {
                                      ValidationState.CreateError("invalid"),
                                  }
                                  : new HashSet<ValidationState>());
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

    private void StubResponseValidator(TestApiResponse response, bool hasErrors)
    {
#pragma warning disable CA2012 // NSubstitute's Returns() consumes the ValueTask via its ambient call router, not a real double-await.
        _responseValidator.Inner.ExecuteAsync(response, Arg.Any<CancellationToken>()).Returns(_ => ValueTask.FromResult(StubValidationResult(hasErrors)));
#pragma warning restore CA2012
    }

    internal sealed class TestAddressValidationRequest : AbstractAddressValidationRequest;

    internal sealed class TestApiResponse;

    private sealed class TestAddressValidationService(
        IApiRequestAdapter<TestAddressValidationRequest, TestApiResponse> requestAdapter,
        IApiResponseMapper<TestApiResponse> responseMapper,
        IValidator<TestAddressValidationRequest> requestValidator,
        IValidator<TestApiResponse> responseValidator)
        : AbstractAddressValidationService<TestAddressValidationRequest, TestApiResponse>(requestAdapter, responseMapper, requestValidator, responseValidator);

    private sealed class TestRequestValidator : AbstractAddressValidationRequestValidator<TestAddressValidationRequest>
    {
        protected override string ProviderName => "Test";

        protected override FrozenSet<CountryCode> SupportedCountries => new[]
        {
            CountryCode.US,
        }.ToFrozenSet();
    }

    private sealed class TestResponseValidator : AbstractValidator<TestApiResponse>
    {
        internal IValidator<TestApiResponse> Inner { get; } = Substitute.For<IValidator<TestApiResponse>>();

        protected override async ValueTask<bool> PreValidateAsync(TestApiResponse instance, ISet<ValidationState> results, CancellationToken cancellationToken = default)
        {
            IValidationResult result = await Inner.ExecuteAsync(instance, cancellationToken).ConfigureAwait(false);
            foreach ( ValidationState state in result.Errors )
            {
                results.Add(state);
            }

            foreach ( ValidationState state in result.Warnings )
            {
                results.Add(state);
            }

            return true;
        }
    }
}
