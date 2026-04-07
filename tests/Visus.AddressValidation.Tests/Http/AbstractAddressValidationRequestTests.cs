namespace Visus.AddressValidation.Tests.Http;

using Abstractions;
using AddressValidation.Http;
using AwesomeAssertions;

#pragma warning disable MA0048
internal sealed class TestAddressValidationRequest : AbstractAddressValidationRequest;
#pragma warning restore MA0048

internal sealed class AbstractAddressValidationRequestTests
{
    [Test]
    public void AddressLines_IsCaseInsensitive()
    {
        TestAddressValidationRequest request = new();
        request.AddressLines.Add("123 Main St");
        request.AddressLines.Add("123 MAIN ST");

        request.AddressLines.Should().HaveCount(1);
    }

    [Test]
    [Arguments(CountryCode.MC)]
    [Arguments(CountryCode.SG)]
    [Arguments(CountryCode.VA)]
    public void Country_CityStates_NullifiesStateOrProvince(CountryCode cityState)
    {
        TestAddressValidationRequest request = new()
        {
            StateOrProvince = "SomeState",
            Country = cityState,
        };

        request.StateOrProvince.Should().BeNull();
        request.Country.Should().Be(cityState);
    }

    [Test]
    [Arguments(CountryCode.AE)]
    [Arguments(CountryCode.HK)]
    [Arguments(CountryCode.QA)]
    public void Country_NoPostalCodeCountries_NullifiesPostalCode(CountryCode country)
    {
        TestAddressValidationRequest request = new()
        {
            Country = country,
        };
        request.PostalCode = "12345";

        request.PostalCode.Should().BeNull();
    }

    [Test]
    public void Country_RegularCountry_PreservesAllFields()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.US,
            StateOrProvince = "TX",
            PostalCode = "75001",
            CityOrTown = "Dallas",
        };
        request.AddressLines.Add("123 Main St");

        request.Country.Should().Be(CountryCode.US);
        request.StateOrProvince.Should().Be("TX");
        request.PostalCode.Should().Be("75001");
        request.CityOrTown.Should().Be("Dallas");
        request.AddressLines.Should().Contain("123 Main St");
    }

    [Test]
    public void Country_SetToNull_IsNoOp()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.US,
        };
        request.Country = null;

        request.Country.Should().Be(CountryCode.US);
    }

    [Test]
    [Arguments(CountryCode.GU, "GU")]
    [Arguments(CountryCode.PR, "PR")]
    [Arguments(CountryCode.VI, "VI")]
    public void Country_UsTerritories_MapsToUs(CountryCode territory, string expectedState)
    {
        TestAddressValidationRequest request = new()
        {
            Country = territory,
        };

        request.Country.Should().Be(CountryCode.US);
        request.StateOrProvince.Should().Be(expectedState);
    }

    [Test]
    public void Country_ZZ_SetsCountryToNull()
    {
        TestAddressValidationRequest request = new()
        {
            Country = CountryCode.ZZ,
        };

        request.Country.Should().BeNull();
    }

    [Test]
    public void PostalCode_WhenCountryIsNull_SetsNormally()
    {
        TestAddressValidationRequest request = new()
        {
            PostalCode = "12345",
        };

        request.PostalCode.Should().Be("12345");
    }

    [Test]
    public void StateOrProvince_WhenCountryIsNull_SetsNormally()
    {
        TestAddressValidationRequest request = new()
        {
            StateOrProvince = "CA",
        };

        request.StateOrProvince.Should().Be("CA");
    }
}
