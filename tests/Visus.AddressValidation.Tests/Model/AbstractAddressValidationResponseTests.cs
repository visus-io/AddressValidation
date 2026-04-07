namespace Visus.AddressValidation.Tests.Model;

using Abstractions;
using AddressValidation.Model;
using AddressValidation.Validation;
using AutoFixture;
using AwesomeAssertions;
using NSubstitute;

#pragma warning disable MA0048
internal sealed class TestAddressValidationResponse : AbstractAddressValidationResponse
#pragma warning restore MA0048
{
    public TestAddressValidationResponse(IValidationResult? validationResult = null)
        : base(validationResult)
    {
    }
}

internal sealed class AbstractAddressValidationResponseTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Constructor_NoValidationResult_LeavesEmptyCollections()
    {
        TestAddressValidationResponse response = new();

        response.Errors.Should().BeEmpty();
        response.Warnings.Should().BeEmpty();
        response.AddressLines.Should().BeEmpty();
        response.Suggestions.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WithValidationResult_PopulatesErrorsAndWarnings()
    {
        IValidationResult validationResult = Substitute.For<IValidationResult>();
        validationResult.Errors.Returns(new HashSet<ValidationState>
        {
            ValidationState.CreateError(_fixture.Create<string>()),
        });
        validationResult.Warnings.Returns(new HashSet<ValidationState>
        {
            ValidationState.CreateWarning(_fixture.Create<string>()),
        });

        TestAddressValidationResponse response = new(validationResult);

        response.Errors.Should().HaveCount(1);
        response.Warnings.Should().HaveCount(1);
    }

    [Test]
    public void Equals_DifferentFields_ReturnsFalse()
    {
        TestAddressValidationResponse left = new()
        {
            CityOrTown = _fixture.Create<string>(),
            Country = CountryCode.US,
        };

        TestAddressValidationResponse right = new()
        {
            CityOrTown = _fixture.Create<string>(),
            Country = CountryCode.US,
        };

        left.Equals(right).Should().BeFalse();
        ( left != right ).Should().BeTrue();
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        TestAddressValidationResponse response = new()
        {
            Country = CountryCode.US,
        };

#pragma warning disable CA1508
        response.Equals(null).Should().BeFalse();
#pragma warning restore CA1508
    }

    [Test]
    public void Equals_SameFields_ReturnsTrue()
    {
        string city = _fixture.Create<string>();
        string state = _fixture.Create<string>();
        string postalCode = _fixture.Create<string>();
        string addressLine = _fixture.Create<string>();

        TestAddressValidationResponse left = new()
        {
            AddressLines = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                addressLine.ToUpperInvariant(),
            },
            CityOrTown = city.ToUpperInvariant(),
            Country = CountryCode.US,
            PostalCode = postalCode,
            StateOrProvince = state.ToUpperInvariant(),
        };

        TestAddressValidationResponse right = new()
        {
            AddressLines = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                addressLine.ToLowerInvariant(),
            },
            CityOrTown = city.ToLowerInvariant(),
            Country = CountryCode.US,
            PostalCode = postalCode,
            StateOrProvince = state.ToLowerInvariant(),
        };

        left.Equals(right).Should().BeTrue();
        ( left == right ).Should().BeTrue();
    }

    [Test]
    public void Equals_WithObject_ReturnsFalseForNonResponse()
    {
        TestAddressValidationResponse response = new()
        {
            Country = CountryCode.US,
        };

        response.Equals("not a response").Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
        string city = _fixture.Create<string>();
        string state = _fixture.Create<string>();
        string postalCode = _fixture.Create<string>();
        string addressLine = _fixture.Create<string>();

        TestAddressValidationResponse left = new()
        {
            AddressLines = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                addressLine.ToUpperInvariant(),
            },
            CityOrTown = city.ToUpperInvariant(),
            Country = CountryCode.US,
            PostalCode = postalCode,
            StateOrProvince = state.ToUpperInvariant(),
        };

        TestAddressValidationResponse right = new()
        {
            AddressLines = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                addressLine.ToLowerInvariant(),
            },
            CityOrTown = city.ToLowerInvariant(),
            Country = CountryCode.US,
            PostalCode = postalCode,
            StateOrProvince = state.ToLowerInvariant(),
        };

        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Test]
    public void Operator_BothNull_ReturnsTrue()
    {
        TestAddressValidationResponse? left = null;
        TestAddressValidationResponse? right = null;

#pragma warning disable CA1508
        ( left == right ).Should().BeTrue();
#pragma warning restore CA1508
    }

    [Test]
    public void Operator_OneNull_ReturnsFalse()
    {
        TestAddressValidationResponse left = new()
        {
            Country = CountryCode.US,
        };

#pragma warning disable CA1508
        ( left == null ).Should().BeFalse();
        ( null == left ).Should().BeFalse();
#pragma warning restore CA1508
    }
}
