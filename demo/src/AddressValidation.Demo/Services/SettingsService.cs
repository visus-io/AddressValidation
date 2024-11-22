namespace AddressValidation.Demo.Services;

using Abstractions;
using Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Models.Entities;

public sealed class SettingsService(
	IDataProtectionProvider dataProtectionProvider,
	ISettingsRepository settingsRepository) : ISettingsService
{
	private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));

	private readonly ISettingsRepository _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));

	public ValueTask<bool> AddOrUpdateAsync(string key, string? value, bool encrypt = false, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);
		return AddOrUpdateInternalAsync(key, value, encrypt, cancellationToken);
	}

	private async ValueTask<bool> AddOrUpdateInternalAsync(string key, string? value, bool encrypt, CancellationToken cancellationToken)
	{
		if ( !string.IsNullOrWhiteSpace(value) && encrypt )
		{
			IDataProtector protector = _dataProtectionProvider.CreateProtector("AddressValidation.Demo.Settings.V1");
			value = protector.Protect(value);
		}

		bool result;
		if ( !await _settingsRepository.AnyAsync(a => a.Key == key, cancellationToken).ConfigureAwait(false) )
		{
			SettingsModel model = new()
			{
				Key = key,
				Value = value,
				IsEncrypted = encrypt
			};

			result = await _settingsRepository.AddAsync(model, cancellationToken).ConfigureAwait(false);
		}
		else
		{
			result = await _settingsRepository.UpdateAsync(u => u.Key == key,
														   s =>
															   s.SetProperty(p => p.Value, value)
																.SetProperty(p => p.IsEncrypted, encrypt),
														   cancellationToken).ConfigureAwait(false);
		}

		return result;
	}
}
