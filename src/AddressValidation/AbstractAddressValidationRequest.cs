namespace Visus.AddressValidation;

using System.Text.Json.Serialization;
using Abstractions;

/// <summary>
///     Base class for implementing a uniformed address validation request.
/// </summary>
public abstract class AbstractAddressValidationRequest
{
	private CountryCode? _country;

	private string? _postalCode;

	private string? _stateOrProvince;

	/// <summary>
	///     Gets or sets address lines
	/// </summary>
	public ISet<string> AddressLines { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///     Gets or sets the city (town)
	/// </summary>
	public string? CityOrTown { get; set; }

	/// <summary>
	///     Gets or sets the country code
	/// </summary>
	/// <remarks>Refer to <see cref="Country" /> for values.</remarks>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public CountryCode? Country
	{
		get => _country;
		set
		{
			_country = value;

			if ( value is null )
			{
				return;
			}

			if ( AddressGlobals.CityStates.Contains(value.Value) )
			{
				StateOrProvince = null;
			}
		}
	}

	/// <summary>
	///     Gets or sets the residential indicator for the address
	/// </summary>
	/// <remarks>Value may be ignored by some validation services.</remarks>
	public bool? IsResidential { get; set; }

	/// <summary>
	///     Gets or sets the Zip (Postal) Code
	/// </summary>
	/// <remarks>
	///     Value will be dropped for countries that have no concept of a postal code. See
	///     <see cref="AbstractAddressValidationRequestValidator{T}" /> for details.
	/// </remarks>
	public string? PostalCode
	{
		get => _postalCode;
		set
		{
			if ( _country is null )
			{
				_postalCode = value;
				return;
			}

			_postalCode = AddressGlobals.NoPostalCode.Contains(_country.Value)
							  ? null
							  : value;
		}
	}

	/// <summary>
	///     Gets or sets the State (Province)
	/// </summary>
	/// <remarks>
	///     Value will be dropped for countries that are considered city-states. See
	///     <see cref="AbstractAddressValidationRequestValidator{T}" /> for details.
	/// </remarks>
	public string? StateOrProvince
	{
		get => _stateOrProvince;
		set
		{
			if ( _country is null )
			{
				_stateOrProvince = value;
				return;
			}

			_stateOrProvince = AddressGlobals.CityStates.Contains(_country.Value)
								   ? null
								   : value;
		}
	}
}
