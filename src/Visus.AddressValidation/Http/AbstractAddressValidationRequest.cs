namespace Visus.AddressValidation.Http;

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
	/// <remarks>
	///     Refer to <see cref="CountryCode" /> for accepted values. Setting the country to <see cref="CountryCode.GU" />,
	///     <see cref="CountryCode.PR" /> or <see cref="CountryCode.VI" /> will automatically change the country to
	///     <see cref="CountryCode.US" /> and set the appropriate <see cref="StateOrProvince" /> value.
	/// </remarks>
	[JsonConverter(typeof(JsonStringEnumConverter<CountryCode>))]
	public CountryCode? Country
	{
		get => _country;
		set
		{
			switch ( value )
			{
				case null:
					return;
				case CountryCode.GU or CountryCode.PR or CountryCode.VI:
					StateOrProvince = value.ToString();
					_country = CountryCode.US;
					break;
				case CountryCode.ZZ:
					_country = null;
					break;
				default:
					_country = value;
					break;
			}

			if ( Constants.CityStates.Contains(value.Value) )
			{
				StateOrProvince = null;
			}
		}
	}

	/// <summary>
	///     Gets or sets the Zip (Postal) Code
	/// </summary>
	/// <remarks>
	///     Value will be dropped for countries that have no concept of a postal code. Refer to the
	///     <see cref="Constants.NoPostalCode" /> collection for details.
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

			_postalCode = Constants.NoPostalCode.Contains(_country.Value)
							  ? null
							  : value;
		}
	}

	/// <summary>
	///     Gets or sets the State (Province)
	/// </summary>
	/// <remarks>
	///     Value will be dropped for countries that are considered city-states. Refer to the
	///     <see cref="Constants.CityStates" /> collection for details.
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

			_stateOrProvince = Constants.CityStates.Contains(_country.Value)
								   ? null
								   : value;
		}
	}
}
