namespace AddressValidation.Demo.Extensions;

using Configuration;

internal static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSqlite(this IConfigurationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Add(new SqliteConfigurationSource());
    }
}
