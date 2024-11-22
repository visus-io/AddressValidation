#nullable disable

namespace AddressValidation.Demo.Models.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class SettingsModel
{
	[Column(Order = 2)]
	public bool IsEncrypted { get; set; }

	[Column(Order = 0)]
	public string Key { get; set; }

	[Column(Order = 1)]
	public string Value { get; set; }
}
