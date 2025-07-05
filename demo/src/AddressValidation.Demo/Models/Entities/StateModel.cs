namespace AddressValidation.Demo.Models.Entities;

public sealed class StateModel
{
    public string CountryCode { get; set; } = null!;

    public int Id { get; set; }

    public string IsoCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Type { get; set; }
}
