namespace AddressValidation.Demo.Services;

using System.Collections.Concurrent;
using Abstractions;
using Common.Components;

public sealed class LoadingIndicatorService : ILoadingIndicatorService
{
	private readonly ConcurrentDictionary<LoadingIndicator, byte> _indicators = new();

	private Func<bool?> _isLoading = default!;

	private Func<bool?> _isVisible = default!;

	private Func<bool, Task> _setIsLoading = default!;

	private Func<bool, Task> _setIsVisible = default!;

	public LoadingIndicatorService()
	{
		MultipleInstanceMode();
	}

	public bool? IsLoading => _isLoading();

	public bool? IsVisible => _isVisible();

	public Task HideAsync()
	{
		return _setIsVisible(true);
	}

	public Task SetIsLoadingAsync(bool value)
	{
		return _setIsLoading(value);
	}

	public Task ShowAsync()
	{
		return _setIsVisible(false);
	}

	private bool? GetIsLoadingMultiple()
	{
		bool? result = null;
		foreach ( LoadingIndicator indicator in _indicators.Keys )
		{
			switch ( result )
			{
				case null:
					result = indicator.IsLoading;
					break;
				default:
				{
					if ( result != indicator.IsLoading )
					{
						return null;
					}

					break;
				}
			}
		}

		return result;
	}

	private bool? GetIsVisibleMultiple()
	{
		bool? result = null;
		foreach ( LoadingIndicator indicator in _indicators.Keys )
		{
			switch ( result )
			{
				case null:
					result = indicator.IsVisible;
					break;
				default:
				{
					if ( result != indicator.IsVisible )
					{
						return null;
					}

					break;
				}
			}
		}

		return result;
	}

	private void MultipleInstanceMode()
	{
		_isLoading = GetIsLoadingMultiple;
		_isVisible = GetIsVisibleMultiple;
		_setIsLoading = SetIsLoadingMultipleAsync;
		_setIsVisible = SetIsVisibleMultipleAsync;
	}

	private Task SetIsLoadingMultipleAsync(bool value)
	{
		List<Task> tasks = new(_indicators.Count);
		tasks.AddRange(_indicators.Keys.Select(indicator => indicator.SetIsLoading(value)));

		return Task.WhenAll(tasks);
	}

	private Task SetIsVisibleMultipleAsync(bool value)
	{
		List<Task> tasks = new(_indicators.Count);
		tasks.AddRange(_indicators.Keys.Select(indicator => indicator.SetIsVisible(value)));

		return Task.WhenAll(tasks);
	}

	private void SingleInstanceMode(LoadingIndicator loadIndicator)
	{
		_isLoading = () => loadIndicator.IsLoading;
		_isVisible = () => loadIndicator.IsVisible;
	}

	void ILoadingIndicatorService.Subscribe(LoadingIndicator loadIndicator)
	{
		_indicators.TryAdd(loadIndicator, 0);
		if ( _indicators.Count == 1 )
		{
			SingleInstanceMode(loadIndicator);
		}
		else
		{
			MultipleInstanceMode();
		}
	}

	void ILoadingIndicatorService.Unsubscribe(LoadingIndicator loadIndicator)
	{
		_indicators.TryRemove(loadIndicator, out _);
		if ( _indicators.Count == 1 )
		{
			SingleInstanceMode(loadIndicator);
		}
		else
		{
			MultipleInstanceMode();
		}
	}
}
