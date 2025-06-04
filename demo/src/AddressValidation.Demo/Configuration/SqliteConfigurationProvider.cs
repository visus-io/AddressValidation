namespace AddressValidation.Demo.Configuration;

using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;

internal sealed class SqliteConfigurationProvider : ConfigurationProvider
{
    public override void Load()
    {
        string keysDirectory = Path.Join(AppDomain.CurrentDomain.GetData("DataDirectory")!.ToString(), "Keys");
        IDataProtectionProvider provider = DataProtectionProvider.Create(new DirectoryInfo(keysDirectory),
                                                                         config => { config.SetApplicationName("AddressValidation.Demo"); });

        SettingsContextFactory contextFactory = new(NullLoggerFactory.Instance);
        SettingsRepository repository = new(contextFactory, NullLogger<SettingsRepository>.Instance);

        try
        {
            IReadOnlyList<SettingsModel> items = repository.List();

            IDataProtector protector = provider.CreateProtector("AddressValidation.Demo.Settings.V1");

            Data = items.Select(s => new
            {
                s.Key,
                Value = s.IsEncrypted ? protector.Unprotect(s.Value) : s.Value
            }).ToDictionary(k => k.Key, string? (v) => v.Value);
        }
        catch ( SqliteException )
        {
        }
    }
}
