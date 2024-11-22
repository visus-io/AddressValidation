namespace AddressValidation.Demo.Models.Forms.Abstractions;

using System.Text.Json;
using Visus.AddressValidation.Abstractions;
using Visus.AddressValidation.Http;

public abstract class AbstractAddressValidationFormModel<TRequest>
	where TRequest : AbstractAddressValidationRequest, new()
{
	private readonly Dictionary<int, string?> _addressLines = new()
	{
		{ 0, null },
		{ 1, null }
	};

	private readonly JsonSerializerOptions _serializerOptions = new()
	{
		WriteIndented = true
	};

	public TRequest Request { get; } = new();

	public string? AddressLine1
	{
		get => _addressLines[0];
		set
		{
			_addressLines[0] = value;
			UpdateAddressLines();
		}
	}

	public string? AddressLine2
	{
		get => _addressLines[1];
		set
		{
			_addressLines[1] = value;
			UpdateAddressLines();
		}
	}

	public string? CityOrTown
	{
		get => Request.CityOrTown;
		set => Request.CityOrTown = value;
	}

	public string? Country
	{
		get => Request.Country.ToString();
		set
		{
			if ( string.IsNullOrWhiteSpace(value) )
			{
				Request.Country = null;
				return;
			}

			if ( Enum.TryParse(value, true, out CountryCode result) )
			{
				Request.Country = result;
				return;
			}

			Request.Country = null;
		}
	}

	public string? PostalCode
	{
		get => Request.PostalCode;
		set => Request.PostalCode = value;
	}

	public string? StateOrProvince
	{
		get => Request.StateOrProvince;
		set => Request.StateOrProvince = value;
	}

	public string ToJson()
	{
		return JsonSerializer.Serialize(Request, _serializerOptions);
	}

	private void UpdateAddressLines()
	{
		Request.AddressLines.Clear();

		foreach ( ( int _, string? value ) in _addressLines.OrderBy(o => o.Key) )
		{
			if ( string.IsNullOrWhiteSpace(value) )
			{
				continue;
			}

			Request.AddressLines.Add(value);
		}
	}
}
