namespace AddressValidation.Demo.Configuration;

internal sealed class SqliteConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new SqliteConfigurationProvider();
    }
}
