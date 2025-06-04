namespace AddressValidation.Demo.Infrastructure.Repositories;

using Abstractions;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public sealed class SettingsRepository(
    IDbContextFactory<SettingsContext> contextFactory,
    ILogger<SettingsRepository> logger) : SqlRepository<SettingsModel, SettingsContext>(contextFactory, logger), ISettingsRepository
{
    private readonly IDbContextFactory<SettingsContext> _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

    public IReadOnlyList<SettingsModel> List()
    {
        using SettingsContext context = _contextFactory.CreateDbContext();
        return context.Set<SettingsModel>().ToList().AsReadOnly();
    }
}
