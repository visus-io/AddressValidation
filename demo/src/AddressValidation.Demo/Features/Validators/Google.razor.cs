namespace AddressValidation.Demo.Features.Validators;

using Abstractions;
using Configuration;
using Models.Forms;
using Radzen;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Integration.Google.Http;
using Constants = Visus.AddressValidation.Integration.Google.Constants;

public partial class Google : AbstractValidatorComponent<GoogleAddressValidationRequest, GoogleAddressValidationFormModel>
{
    private readonly GoogleApiSettingsFormModel _settingsFormModel = new();

    protected override IEnumerable<CountryCode> InitializeCountries()
    {
        return Constants.SupportedCountries;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _settingsFormModel.PrivateKey = Configuration.GetValue<string>(Constants.PrivateKeyConfigurationKey);
        _settingsFormModel.ProjectId = Configuration.GetValue<string>(Constants.ProjectIdConfigurationKey);
        _settingsFormModel.ServiceAccountEmailAddress = Configuration.GetValue<string>(Constants.ServiceAccountEmailConfigurationKey);
    }

    private async Task OnSettingsFormSubmitAsync()
    {
        await SettingsLoadingIndicator.ShowAsync();

        try
        {
            bool[] results =
            [
                await SettingsService.AddOrUpdateAsync(Constants.PrivateKeyConfigurationKey, _settingsFormModel.PrivateKey, true),
                await SettingsService.AddOrUpdateAsync(Constants.ProjectIdConfigurationKey, _settingsFormModel.ProjectId),
                await SettingsService.AddOrUpdateAsync(Constants.ServiceAccountEmailConfigurationKey, _settingsFormModel.ServiceAccountEmailAddress)
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
