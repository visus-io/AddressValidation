namespace AddressValidation.Demo.Infrastructure;

using Microsoft.EntityFrameworkCore;

public sealed class GeoContextFactory(ILoggerFactory loggerFactory) : IDbContextFactory<GeoContext>
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public GeoContext CreateDbContext()
    {
        DbContextOptionsBuilder<GeoContext> options = new();

        string database = Path.Join(AppDomain.CurrentDomain.GetData("DataDirectory")!.ToString(), "countries+states.db");

        options.UseSqlite($"Data Source={database}");

        options.UseLoggerFactory(_loggerFactory);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        return new GeoContext(options.Options);
    }
}
