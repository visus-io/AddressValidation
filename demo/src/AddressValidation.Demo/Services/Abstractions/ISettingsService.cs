namespace AddressValidation.Demo.Services.Abstractions;

public interface ISettingsService
{
    ValueTask<bool> AddOrUpdateAsync(string key, string? value, bool encrypt = false, CancellationToken cancellationToken = default);
}
