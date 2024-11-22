namespace AddressValidation.Demo.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

public sealed class StateConfig : IEntityTypeConfiguration<StateModel>
{
	public void Configure(EntityTypeBuilder<StateModel> builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.HasKey(p => p.Id);

		builder.ToTable("states");

		builder.Property(p => p.CountryCode).HasColumnName("country_code").IsRequired().HasMaxLength(2);
		builder.Property(p => p.Id).IsRequired();
		builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
		builder.Property(p => p.IsoCode).HasColumnName("iso2").IsRequired().HasMaxLength(255);
	}
}
