namespace AddressValidation.Demo.Infrastructure;

using Configuration;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public sealed class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options)
{
    public DbSet<CountryModel> Countries { get; set; }

    public DbSet<StateModel> States { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new CountryConfig());
        modelBuilder.ApplyConfiguration(new StateConfig());
    }
}
