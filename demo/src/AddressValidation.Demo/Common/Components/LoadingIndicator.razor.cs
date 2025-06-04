namespace AddressValidation.Demo.Common.Components;

using System.Diagnostics.CodeAnalysis;
using Services.Abstractions;

public sealed partial class LoadingIndicator : ComponentBase, IDisposable
{
    private ILoadingIndicatorService? _loadingIndicatorService;

    ~LoadingIndicator()
    {
        Dispose();
    }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    [SuppressMessage("Usage", "BL0007:Component parameters should be auto properties")]
    public ILoadingIndicatorService? LoadingIndicatorService
    {
        get => _loadingIndicatorService;
        set
        {
            if ( value != _loadingIndicatorService )
            {
                _loadingIndicatorService?.Unsubscribe(this);
            }

            _loadingIndicatorService = value;
            _loadingIndicatorService?.Subscribe(this);
        }
    }

    public void Dispose()
    {
        _loadingIndicatorService = null;
        GC.SuppressFinalize(this);
    }

    public Task HideAsync()
    {
        return SetIsVisible(false);
    }

    public Task ShowAsync()
    {
        return SetIsVisible(true);
    }

    internal async Task SetIsLoading(bool value)
    {
        if ( IsLoading == value )
        {
            return;
        }

        IsLoading = value;
        await InvokeAsync(StateHasChanged);
    }

    internal async Task SetIsVisible(bool value)
    {
        if ( IsVisible == value )
        {
            return;
        }

        IsVisible = value;
        await InvokeAsync(StateHasChanged);
    }
}
