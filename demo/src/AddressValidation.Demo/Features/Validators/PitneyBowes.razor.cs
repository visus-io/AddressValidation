namespace AddressValidation.Demo.Features.Validators;

using Abstractions;
using Configuration;
using Models.Forms;
using Radzen;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Integration.PitneyBowes;
using Visus.AddressValidation.Integration.PitneyBowes.Http;

public partial class PitneyBowes : AbstractValidatorComponent<PitneyBowesAddressValidationRequest, PitneyBowesAddressValidationFormModel>
{
    private readonly Dictionary<string, ClientEnvironment> _clientEnvironments = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(ClientEnvironment.DEVELOPMENT)] = ClientEnvironment.DEVELOPMENT,
        [nameof(ClientEnvironment.PRODUCTION)] = ClientEnvironment.PRODUCTION
    };

    private readonly PitneyBowesApiSettingsFormModel _settingsFormModel = new();

    protected override IEnumerable<CountryCode> InitializeCountries()
    {
        return Constants.SupportedCountries;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _settingsFormModel.DeveloperId = Configuration.GetValue<string>(Constants.DeveloperIdConfigurationKey);

        string? clientEnvironmentValue = Configuration.GetValue<string>(Constants.ClientEnvironmentConfigurationKey);
        if ( !Enum.TryParse(clientEnvironmentValue, out ClientEnvironment clientEnvironment) )
        {
            _settingsFormModel.ClientEnvironment = ClientEnvironment.DEVELOPMENT;
        }

        _settingsFormModel.ClientEnvironment = clientEnvironment;
        _settingsFormModel.ApiKey = Configuration.GetValue<string>(Constants.ApiKeyConfigurationKey);
        _settingsFormModel.ApiSecret = Configuration.GetValue<string>(Constants.ApiSecretConfigurationKey);
    }

    private async Task OnSettingsFormSubmitAsync()
    {
        await SettingsLoadingIndicator.ShowAsync();

        try
        {
            bool[] results =
            [
                await SettingsService.AddOrUpdateAsync(Constants.DeveloperIdConfigurationKey, _settingsFormModel.DeveloperId),
                await SettingsService.AddOrUpdateAsync(Constants.ClientEnvironmentConfigurationKey, _settingsFormModel.ClientEnvironment.ToString()),
                await SettingsService.AddOrUpdateAsync(Constants.ApiKeyConfigurationKey, _settingsFormModel.ApiKey, true),
                await SettingsService.AddOrUpdateAsync(Constants.ApiSecretConfigurationKey, _settingsFormModel.ApiSecret, true)
            ];

            if ( results.All(a => a) )
            {
                NotificationService.Notify(NotificationSeverity.Success, "Configuration Updated");

                // remark: refresh underlying IConfiguration provider for validation services
                if ( Configuration is IConfigurationRoot configurationRoot )
                {
                    IConfigurationProvider? provider = configurationRoot.Providers.FirstOrDefault(f => f is SqliteConfigurationProvider);
                    provider?.Load();
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Configuration Update Failed");
            }
        }
        finally
        {
            await SettingsLoadingIndicator.HideAsync();
        }

        await InvokeAsync(StateHasChanged);
    }
}
