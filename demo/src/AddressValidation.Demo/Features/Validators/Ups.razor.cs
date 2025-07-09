namespace AddressValidation.Demo.Features.Validators;

using Abstractions;
using Configuration;
using Models.Forms;
using Radzen;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Integration.Ups;
using Visus.AddressValidation.Integration.Ups.Http;

public partial class Ups : AbstractValidatorComponent<UpsAddressValidationRequest, UpsAddressValidationFormModel>
{
    private readonly Dictionary<string, ClientEnvironment> _clientEnvironments = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(ClientEnvironment.DEVELOPMENT)] = ClientEnvironment.DEVELOPMENT,
        [nameof(ClientEnvironment.PRODUCTION)] = ClientEnvironment.PRODUCTION
    };

    private readonly OAuthApiSettingsFormModel _settingsFormModel = new();

    protected override IEnumerable<CountryCode> InitializeCountries()
    {
        return Constants.SupportedCountries;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _settingsFormModel.AccountNumber = Configuration.GetValue<string>(Constants.AccountNumberConfigurationKey);

        string? clientEnvironmentValue = Configuration.GetValue<string>(Constants.ClientEnvironmentConfigurationKey);
        if ( !Enum.TryParse(clientEnvironmentValue, out ClientEnvironment clientEnvironment) )
        {
            _settingsFormModel.ClientEnvironment = ClientEnvironment.DEVELOPMENT;
        }

        _settingsFormModel.ClientEnvironment = clientEnvironment;
        _settingsFormModel.ClientId = Configuration.GetValue<string>(Constants.ClientIdConfigurationKey);
        _settingsFormModel.ClientSecret = Configuration.GetValue<string>(Constants.ClientSecretConfigurationKey);
    }

    private async Task OnSettingsFormSubmitAsync()
    {
        await SettingsLoadingIndicator.ShowAsync();

        try
        {
            bool[] results =
            [
                await SettingsService.AddOrUpdateAsync(Constants.AccountNumberConfigurationKey, _settingsFormModel.AccountNumber),
                await SettingsService.AddOrUpdateAsync(Constants.ClientEnvironmentConfigurationKey, _settingsFormModel.ClientEnvironment.ToString()),
                await SettingsService.AddOrUpdateAsync(Constants.ClientIdConfigurationKey, _settingsFormModel.ClientId, true),
                await SettingsService.AddOrUpdateAsync(Constants.ClientSecretConfigurationKey, _settingsFormModel.ClientSecret, true)
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
