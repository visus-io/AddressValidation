namespace AddressValidation.Demo.Models.Forms;

using Visus.AddressValidation.Abstractions;

public class UpsApiSettingsFormModel
{
	public string? AccountNumber { get; set; }

	public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

	public string? ClientId { get; set; }

	public string? ClientSecret { get; set; }
}
