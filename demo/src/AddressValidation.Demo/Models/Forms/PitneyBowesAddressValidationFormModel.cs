namespace AddressValidation.Demo.Models.Forms;

using Abstractions;
using Visus.AddressValidation.Integration.PitneyBowes.Http;

public sealed class PitneyBowesAddressValidationFormModel : AbstractAddressValidationFormModel<PitneyBowesAddressValidationRequest>
{
	public bool IncludeSuggestions
	{
		get => Request.IncludeSuggestions;
		set => Request.IncludeSuggestions = value;
	}
}
