namespace AddressValidation.Demo.Models.Entities;

public sealed class CountryModel
{
    public int Id { get; set; }

    public string IsoCode { get; set; } = null!;

    public string Name { get; set; } = null!;
}
