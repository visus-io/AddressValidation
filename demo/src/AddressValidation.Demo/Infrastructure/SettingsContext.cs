namespace AddressValidation.Demo.Infrastructure;

using Configuration;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public sealed class SettingsContext(DbContextOptions<SettingsContext> options) : DbContext(options)
{
	public DbSet<SettingsModel> Config { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ArgumentNullException.ThrowIfNull(modelBuilder);

		modelBuilder.ApplyConfiguration(new SettingsConfig());
	}
}
