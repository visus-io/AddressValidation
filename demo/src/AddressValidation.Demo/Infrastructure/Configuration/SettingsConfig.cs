namespace AddressValidation.Demo.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

public sealed class SettingsConfig : IEntityTypeConfiguration<SettingsModel>
{
	public void Configure(EntityTypeBuilder<SettingsModel> builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.HasKey(p => p.Key);

		builder.ToTable("Settings");

		builder.Property(p => p.Key).ValueGeneratedNever().IsRequired().HasMaxLength(255).IsUnicode(false);
		builder.Property(p => p.Value).IsUnicode().HasMaxLength(int.MaxValue);
		builder.Property(p => p.IsEncrypted).IsRequired().HasDefaultValue(false);
	}
}
