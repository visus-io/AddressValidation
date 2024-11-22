namespace AddressValidation.Demo.Infrastructure;

using Microsoft.EntityFrameworkCore;

public sealed class SettingsContextFactory(ILoggerFactory loggerFactory) : IDbContextFactory<SettingsContext>
{
	private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

	public SettingsContext CreateDbContext()
	{
		DbContextOptionsBuilder<SettingsContext> options = new();

		string database = Path.Join(AppDomain.CurrentDomain.GetData("DataDirectory")!.ToString(), "settings.db");

		options.UseSqlite($"Data Source={database}");

		options.UseLoggerFactory(_loggerFactory);
		options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

		return new SettingsContext(options.Options);
	}
}
