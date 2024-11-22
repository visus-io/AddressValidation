namespace AddressValidation.Demo.Models.Forms;

using Visus.AddressValidation.Abstractions;

public class PitneyBowesApiSettingsFormModel
{
	public string? ApiKey { get; set; }

	public string? ApiSecret { get; set; }

	public ClientEnvironment ClientEnvironment { get; set; } = ClientEnvironment.DEVELOPMENT;

	public string? DeveloperId { get; set; }
}
