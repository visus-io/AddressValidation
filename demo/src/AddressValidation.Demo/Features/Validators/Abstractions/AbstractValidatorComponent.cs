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

public abstract class AbstractValidatorComponent<TValidationRequest, TValidationForm> : ComponentBase
    where TValidationRequest : AbstractAddressValidationRequest, new()
    where TValidationForm : AbstractAddressValidationFormModel<TValidationRequest>, new()
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    protected TValidationForm AddressValidationFormModel { get; } = new();

    [Inject]
    protected IConfiguration Configuration { get; set; } = default!;

    protected IReadOnlyDictionary<string, string> Countries { get; private set; } = new Dictionary<string, string>();

    [Inject]
    protected IGeographyService GeographyService { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    protected NotificationService NotificationService { get; set; } = default!;

    protected IReadOnlyDictionary<string, string> Provinces { get; private set; } = new Dictionary<string, string>();

    protected MarkupString? RequestJson { get; private set; }

    protected MarkupString? ResponseJson { get; private set; }

    protected LoadingIndicator ResultsLoadingIndicator { get; set; } = default!;

    protected LoadingIndicator SettingsLoadingIndicator { get; set; } = default!;

    [Inject]
    protected ISettingsService SettingsService { get; set; } = default!;

    protected LoadingIndicator ValidateLoadingIndicator { get; set; } = default!;

    [Inject]
    protected IAddressValidationService<TValidationRequest> ValidationService { get; set; } = default!;

    protected virtual IEnumerable<CountryCode> InitializeCountries()
    {
        yield break;
    }

    protected async Task OnAddressValidationFormSubmitAsync()
    {
        await ValidateLoadingIndicator.ShowAsync();
        await ResultsLoadingIndicator.ShowAsync();

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
            await ResultsLoadingIndicator.HideAsync();
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
                   .ContinueWith(t => { Provinces = t.Result; }, CancellationToken.None, TaskContinuationOptions.LongRunning, TaskScheduler.Default);
    }

    protected override void OnInitialized()
    {
        AddressValidationFormModel.Country = CountryCode.US.ToString();
    }

    private Task LoadCountriesAsync()
    {
        return Task.Run(async () => await GeographyService.ListCountriesAsDictionaryAsync())
                   .ContinueWith(t => { Countries = t.Result; }, CancellationToken.None, TaskContinuationOptions.LongRunning, TaskScheduler.Default);
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
