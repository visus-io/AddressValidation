namespace Visus.AddressValidation.Tests.Http;

using System.Text;
using AddressValidation.Http;
using AwesomeAssertions;

internal sealed class BasicAuthenticationHeaderValueTests
{
    [Test]
    public void Constructor_EncodesCredentials()
    {
        BasicAuthenticationHeaderValue header = new("user", "pass");

        string expected = Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass"));

        header.Scheme.Should().Be("Basic");
        header.Parameter.Should().Be(expected);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void Constructor_NullOrEmptyPassword_UsesEmptyPassword(string? password)
    {
        BasicAuthenticationHeaderValue header = new("user", password);

        string expected = Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));

        header.Parameter.Should().Be(expected);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void Constructor_NullOrWhitespaceUsername_Throws(string? username)
    {
        Action act = () => _ = new BasicAuthenticationHeaderValue(username!, "pass");

        act.Should().Throw<ArgumentException>();
    }
}
