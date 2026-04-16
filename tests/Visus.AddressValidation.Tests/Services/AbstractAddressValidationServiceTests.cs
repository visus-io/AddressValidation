namespace Visus.AddressValidation.Tests.Services;

using System.Collections.Frozen;
using Abstractions;
using AddressValidation.Http;
using AddressValidation.Model;
using AddressValidation.Services;
using AddressValidation.Validation;
using AutoFixture;
using AwesomeAssertions;
using Http;
using NSubstitute;

#pragma warning disable MA0048
internal sealed class TestApiResponse : IApiResponse
#pragma warning restore MA0048
{
    public IAddressValidationResponse ToAddressValidationResponse(IValidationResult? validationResult = null)
    {
        return new EmptyAddressValidationResponse(validationResult);
    }
}

#pragma warning disable MA0048
internal sealed class TestRequestValidator : AbstractAddressValidationRequestValidator<TestAddressValidationRequest>
#pragma warning restore MA0048
{
    protected override string ProviderName => "Test";

    protected override FrozenSet<CountryCode> SupportedCountries => new HashSet<CountryCode>
    {
        CountryCode.US,
        CountryCode.CA,
    }.ToFrozenSet();
}

#pragma warning disable MA0048
internal sealed class TestResponseValidator : AbstractValidator<TestApiResponse>;
#pragma warning restore MA0048
#pragma warning disable MA0048
internal sealed class TestAddressValidationService : AbstractAddressValidationService<TestAddressValidationRequest, TestApiResponse>
#pragma warning restore MA0048
{
    private readonly Func<TestAddressValidationRequest, CancellationToken, Task<TestApiResponse?>> _sendFunc;

    public TestAddressValidationService(
        IValidator<TestAddressValidationRequest> requestValidator,
        IValidator<TestApiResponse> responseValidator,
        Func<TestAddressValidationRequest, CancellationToken, Task<TestApiResponse?>> sendFunc)
        : base(requestValidator, responseValidator)
    {
        _sendFunc = sendFunc;
    }

    protected override Task<TestApiResponse?> SendAsync(TestAddressValidationRequest request, CancellationToken cancellationToken)
    {
        return _sendFunc(request, cancellationToken);
    }
}

internal sealed class AbstractAddressValidationServiceTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Constructor_InvalidRequestValidator_ThrowsInvalidImplementationException()
    {
        IValidator<TestAddressValidationRequest> badValidator = Substitute.For<IValidator<TestAddressValidationRequest>>();

        Action act = () => _ = new TestAddressValidationService(
                               badValidator,
                               new TestResponseValidator(),
                               (_, _) => Task.FromResult<TestApiResponse?>(null));

        act.Should().Throw<InvalidImplementationException>();
    }

    [Test]
    public void Constructor_NullRequestValidator_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(
                               null!,
                               new TestResponseValidator(),
                               (_, _) => Task.FromResult<TestApiResponse?>(null));

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_NullResponseValidator_ThrowsArgumentNullException()
    {
        Action act = () => _ = new TestAddressValidationService(
                               new TestRequestValidator(),
                               null!,
                               (_, _) => Task.FromResult<TestApiResponse?>(null));

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task ValidateAsync_NullRequest_ThrowsArgumentNullException()
    {
        TestAddressValidationService service = new(
            new TestRequestValidator(),
            new TestResponseValidator(),
            (_, _) => Task.FromResult<TestApiResponse?>(null));

        Func<Task> act = () => service.ValidateAsync(null!);

        await act.Should().ThrowExactlyAsync<ArgumentNullException>().ConfigureAwait(false);
    }

    [Test]
    public async Task ValidateAsync_RequestValidationErrors_ReturnsEmptyResponse()
    {
        TestAddressValidationService service = new(
            new TestRequestValidator(),
            new TestResponseValidator(),
            (_, _) => Task.FromResult<TestApiResponse?>(new TestApiResponse()));

        TestAddressValidationRequest request = new();

        IAddressValidationResponse? result = await service.ValidateAsync(request).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ValidateAsync_SendReturnsNull_ReturnsNull()
    {
        TestAddressValidationService service = new(
            new TestRequestValidator(),
            new TestResponseValidator(),
            (_, _) => Task.FromResult<TestApiResponse?>(null));

        IAddressValidationResponse? result = await service.ValidateAsync(CreateValidRequest()).ConfigureAwait(false);

        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_ValidRequest_ReturnsResponse()
    {
        TestAddressValidationService service = new(
            new TestRequestValidator(),
            new TestResponseValidator(),
            (_, _) => Task.FromResult<TestApiResponse?>(new TestApiResponse()));

        IAddressValidationResponse? result = await service.ValidateAsync(CreateValidRequest()).ConfigureAwait(false);

        result.Should().NotBeNull();
    }

    private TestAddressValidationRequest CreateValidRequest()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.US,
            CityOrTown = _fixture.Create<string>(),
            StateOrProvince = _fixture.Create<string>(),
            PostalCode = _fixture.Create<string>(),
        };
        request.AddressLines.Add(_fixture.Create<string>());
        return request;
    }
}
