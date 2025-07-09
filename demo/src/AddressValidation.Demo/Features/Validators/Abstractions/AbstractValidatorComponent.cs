namespace AddressValidation.Demo.Features.Validators.Abstractions;

using System.Text.Json;
using Common.Components;
using Microsoft.JSInterop;
using Models.Forms.Abstractions;
using Radzen;
using Services.Abstractions;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Http;
using Visus.AddressValidation.Model;
using Visus.AddressValidation.Services;

public abstract class AbstractValidatorComponent<TValidationRequest, TValidationFormModel> : ComponentBase
    where TValidationRequest : AbstractAddressValidationRequest, new()
    where TValidationFormModel : AbstractAddressValidationFormModel<TValidationRequest>, new()
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    protected TValidationFormModel AddressValidationFormModel { get; } = new();

    protected bool IsCityOrTownDisabled
    {
        get
        {
            if ( string.IsNullOrWhiteSpace(AddressValidationFormModel.CityOrTown) ||
                 string.IsNullOrWhiteSpace(AddressValidationFormModel.StateOrProvince) )
            {
                return false;
            }

            string? stateOrProvinceName = Provinces!.GetValueOrDefault(AddressValidationFormModel.StateOrProvince);

            return !string.IsNullOrWhiteSpace(stateOrProvinceName)
                && string.Equals(stateOrProvinceName,
                                 AddressValidationFormModel.CityOrTown,
                                 StringComparison.OrdinalIgnoreCase);
        }
    }

    [Inject]
    protected IConfiguration Configuration { get; set; } = null!;

    protected IReadOnlyDictionary<string, string> Countries { get; private set; } = new Dictionary<string, string>();

    [Inject]
    protected IGeographyService GeographyService { get; set; } = null!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    protected NotificationService NotificationService { get; set; } = null!;

    protected IReadOnlyDictionary<string, string> Provinces { get; private set; } = new Dictionary<string, string>();

    protected MarkupString? RequestJson { get; private set; }

    protected LoadingIndicator RequestLoadingIndicator { get; set; } = null!;

    protected MarkupString? ResponseJson { get; private set; }

    protected LoadingIndicator ResponseLoadingIndicator { get; set; } = null!;

    protected LoadingIndicator SettingsLoadingIndicator { get; set; } = null!;

    [Inject]
    protected ISettingsService SettingsService { get; set; } = null!;

    protected LoadingIndicator ValidateLoadingIndicator { get; set; } = null!;

    [Inject]
    protected IAddressValidationService<TValidationRequest> ValidationService { get; set; } = null!;

    protected virtual IEnumerable<CountryCode> InitializeCountries()
    {
        yield break;
    }

    protected async Task OnAddressValidationFormSubmitAsync()
    {
        await ValidateLoadingIndicator.ShowAsync();
        await RequestLoadingIndicator.ShowAsync();
        await ResponseLoadingIndicator.ShowAsync();

        try
        {
            await RenderRequestJsonAsync();

            IAddressValidationResponse? result = await ValidationService.ValidateAsync(AddressValidationFormModel.Request);
            if ( result is not null )
            {
                await RenderResponseJsonAsync(result);
            }
        }
        finally
        {
            await ValidateLoadingIndicator.HideAsync();
            await RequestLoadingIndicator.HideAsync();
            await ResponseLoadingIndicator.HideAsync();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if ( !firstRender )
        {
            return;
        }

        await ValidateLoadingIndicator.ShowAsync();

        try
        {
            await Task.WhenAll(LoadCountriesAsync(), OnCountryChangedAsync());
        }
        finally
        {
            await ValidateLoadingIndicator.HideAsync();
        }
    }

    protected Task OnCountryChangedAsync()
    {
        if ( string.IsNullOrWhiteSpace(AddressValidationFormModel.Country) )
        {
            return Task.CompletedTask;
        }

        return Task.Run(async () => await GeographyService.ListProvincesAsDictionaryAsync(AddressValidationFormModel.Country))
                   .ContinueWith(t =>
                                 {
                                     Provinces = t.Result;
                                 }, CancellationToken.None, TaskContinuationOptions.LongRunning, TaskScheduler.Default);
    }

    protected override void OnInitialized()
    {
        AddressValidationFormModel.Country = nameof(CountryCode.US);
    }

    protected Task OnProvinceChangedAsync()
    {
        if ( string.IsNullOrWhiteSpace(AddressValidationFormModel.Country) ||
             string.IsNullOrWhiteSpace(AddressValidationFormModel.StateOrProvince) )
        {
            return Task.CompletedTask;
        }

        return Task.Run(async () => await GeographyService.ListAutonomousCitiesAsync(AddressValidationFormModel.Country))
                   .ContinueWith(t =>
                                 {
                                     string? stateOrProvinceName = Provinces.GetValueOrDefault(AddressValidationFormModel.StateOrProvince);
                                     if ( !string.IsNullOrWhiteSpace(stateOrProvinceName) && t.Result.Contains(stateOrProvinceName) )
                                     {
                                         AddressValidationFormModel.CityOrTown = stateOrProvinceName;
                                     }
                                 }, CancellationToken.None, TaskContinuationOptions.LongRunning, TaskScheduler.Default);
    }

    private Task LoadCountriesAsync()
    {
        return Task.Run(async () => await GeographyService.ListCountriesAsDictionaryAsync())
                   .ContinueWith(t =>
                                 {
                                     Countries = t.Result;
                                 }, CancellationToken.None, TaskContinuationOptions.LongRunning, TaskScheduler.Default);
    }

    private async Task RenderRequestJsonAsync()
    {
        string json = AddressValidationFormModel.ToJson();
        string result = await JsRuntime.InvokeAsync<string>("highlightJson", json);

        RequestJson = new MarkupString(result);
    }

    private async Task RenderResponseJsonAsync(IAddressValidationResponse response)
    {
        string json = JsonSerializer.Serialize(response, _serializerOptions);
        string result = await JsRuntime.InvokeAsync<string>("highlightJson", json);

        ResponseJson = new MarkupString(result);
    }
}
