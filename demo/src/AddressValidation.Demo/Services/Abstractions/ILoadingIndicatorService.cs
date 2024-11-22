namespace AddressValidation.Demo.Services.Abstractions;

using Common.Components;

public interface ILoadingIndicatorService
{
	bool? IsLoading { get; }

	bool? IsVisible { get; }

	Task HideAsync();

	Task SetIsLoadingAsync(bool value);

	Task ShowAsync();

	public void Subscribe(LoadingIndicator loadIndicator);

	public void Unsubscribe(LoadingIndicator loadIndicator);
}
