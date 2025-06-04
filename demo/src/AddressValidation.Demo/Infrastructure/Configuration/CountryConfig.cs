namespace AddressValidation.Demo.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

public sealed class CountryConfig : IEntityTypeConfiguration<CountryModel>
{
    public void Configure(EntityTypeBuilder<CountryModel> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(p => p.Id);

        builder.ToTable("countries");

        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.IsoCode).HasColumnName("iso2").IsRequired().HasMaxLength(2);
    }
}
