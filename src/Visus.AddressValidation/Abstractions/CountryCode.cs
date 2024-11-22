// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace Visus.AddressValidation.Abstractions;

using System.ComponentModel;

/// <summary>
///     Enumeration of ISO-3166-1 Alpha 2 Country Codes
/// </summary>
public enum CountryCode
{
	/// <summary>
	///     Unknown
	/// </summary>
	[Description("Unknown")]
	ZZ = 0,

	/// <summary>
	///     Afghanistan
	/// </summary>
	[Description("Afghanistan")]
	AF = 4,

	/// <summary>
	///     Albania
	/// </summary>
	[Description("Albania")]
	AL = 8,

	/// <summary>
	///     Antarctica
	/// </summary>
	[Description("Antarctica")]
	AQ = 10,

	/// <summary>
	///     Algeria
	/// </summary>
	[Description("Algeria")]
	DZ = 12,

	/// <summary>
	///     American Samoa
	/// </summary>
	[Description("American Samoa")]
	AS = 16,

	/// <summary>
	///     Andorra
	/// </summary>
	[Description("Andorra")]
	AD = 20,

	/// <summary>
	///     Angola
	/// </summary>
	[Description("Angola")]
	AO = 24,

	/// <summary>
	///     Antigua and Barbuda
	/// </summary>
	[Description("Antigua and Barbuda")]
	AG = 28,

	/// <summary>
	///     Azerbaijan
	/// </summary>
	[Description("Azerbaijan")]
	AZ = 31,

	/// <summary>
	///     Argentina
	/// </summary>
	[Description("Argentina")]
	AR = 32,

	/// <summary>
	///     Australia
	/// </summary>
	[Description("Australia")]
	AU = 36,

	/// <summary>
	///     Austria
	/// </summary>
	[Description("Austria")]
	AT = 40,

	/// <summary>
	///     Bahamas
	/// </summary>
	[Description("Bahamas")]
	BS = 44,

	/// <summary>
	///     Bahrain
	/// </summary>
	[Description("Bahrain")]
	BH = 48,

	/// <summary>
	///     Bangladesh
	/// </summary>
	[Description("Bangladesh")]
	BD = 50,

	/// <summary>
	///     Armenia
	/// </summary>
	[Description("Armenia")]
	AM = 51,

	/// <summary>
	///     Barbados
	/// </summary>
	[Description("Barbados")]
	BB = 52,

	/// <summary>
	///     Belgium
	/// </summary>
	[Description("Belgium")]
	BE = 56,

	/// <summary>
	///     Bermuda
	/// </summary>
	[Description("Bermuda")]
	BM = 60,

	/// <summary>
	///     Bhutan
	/// </summary>
	[Description("Bhutan")]
	BT = 64,

	/// <summary>
	///     Bolivia (Plurinational State of)
	/// </summary>
	[Description("Bolivia (Plurinational State of)")]
	BO = 68,

	/// <summary>
	///     Bosnia and Herzegovina
	/// </summary>
	[Description("Bosnia and Herzegovina")]
	BA = 70,

	/// <summary>
	///     Botswana
	/// </summary>
	[Description("Botswana")]
	BW = 72,

	/// <summary>
	///     Bouvet Island
	/// </summary>
	[Description("Bouvet Island")]
	BV = 74,

	/// <summary>
	///     Brazil
	/// </summary>
	[Description("Brazil")]
	BR = 76,

	/// <summary>
	///     Belize
	/// </summary>
	[Description("Belize")]
	BZ = 84,

	/// <summary>
	///     British Indian Ocean Territory
	/// </summary>
	[Description("British Indian Ocean Territory")]
	IO = 86,

	/// <summary>
	///     Solomon Islands
	/// </summary>
	[Description("Solomon Islands")]
	SB = 90,

	/// <summary>
	///     Virgin Islands (British)
	/// </summary>
	[Description("Virgin Islands (British)")]
	VG = 92,

	/// <summary>
	///     Brunei Darussalam
	/// </summary>
	[Description("Brunei Darussalam")]
	BN = 96,

	/// <summary>
	///     Bulgaria
	/// </summary>
	[Description("Bulgaria")]
	BG = 100,

	/// <summary>
	///     Myanmar
	/// </summary>
	[Description("Myanmar")]
	MM = 104,

	/// <summary>
	///     Burundi
	/// </summary>
	[Description("Burundi")]
	BI = 108,

	/// <summary>
	///     Belarus
	/// </summary>
	[Description("Belarus")]
	BY = 112,

	/// <summary>
	///     Cambodia
	/// </summary>
	[Description("Cambodia")]
	KH = 116,

	/// <summary>
	///     Cameroon
	/// </summary>
	[Description("Cameroon")]
	CM = 120,

	/// <summary>
	///     Canada
	/// </summary>
	[Description("Canada")]
	CA = 124,

	/// <summary>
	///     Cabo Verde
	/// </summary>
	[Description("Cabo Verde")]
	CV = 132,

	/// <summary>
	///     Cayman Islands
	/// </summary>
	[Description("Cayman Islands")]
	KY = 136,

	/// <summary>
	///     Central African Republic
	/// </summary>
	[Description("Central African Republic")]
	CF = 140,

	/// <summary>
	///     Sri Lanka
	/// </summary>
	[Description("Sri Lanka")]
	LK = 144,

	/// <summary>
	///     Chad
	/// </summary>
	[Description("Chad")]
	TD = 148,

	/// <summary>
	///     Chile
	/// </summary>
	[Description("Chile")]
	CL = 152,

	/// <summary>
	///     China
	/// </summary>
	[Description("China")]
	CN = 156,

	/// <summary>
	///     Taiwan, Province of China
	/// </summary>
	[Description("Taiwan, Province of China")]
	TW = 158,

	/// <summary>
	///     Christmas Island
	/// </summary>
	[Description("Christmas Island")]
	CX = 162,

	/// <summary>
	///     Cocos (Keeling) Islands
	/// </summary>
	[Description("Cocos (Keeling) Islands")]
	CC = 166,

	/// <summary>
	///     Colombia
	/// </summary>
	[Description("Colombia")]
	CO = 170,

	/// <summary>
	///     Comoros
	/// </summary>
	[Description("Comoros")]
	KM = 174,

	/// <summary>
	///     Mayotte
	/// </summary>
	[Description("Mayotte")]
	YT = 175,

	/// <summary>
	///     Congo
	/// </summary>
	[Description("Congo")]
	CG = 178,

	/// <summary>
	///     Congo, Democratic Republic of the
	/// </summary>
	[Description("Congo, Democratic Republic of the")]
	CD = 180,

	/// <summary>
	///     Cook Islands
	/// </summary>
	[Description("Cook Islands")]
	CK = 184,

	/// <summary>
	///     Costa Rica
	/// </summary>
	[Description("Costa Rica")]
	CR = 188,

	/// <summary>
	///     Croatia
	/// </summary>
	[Description("Croatia")]
	HR = 191,

	/// <summary>
	///     Cuba
	/// </summary>
	[Description("Cuba")]
	CU = 192,

	/// <summary>
	///     Cyprus
	/// </summary>
	[Description("Cyprus")]
	CY = 196,

	/// <summary>
	///     Czechia
	/// </summary>
	[Description("Czechia")]
	CZ = 203,

	/// <summary>
	///     Benin
	/// </summary>
	[Description("Benin")]
	BJ = 204,

	/// <summary>
	///     Denmark
	/// </summary>
	[Description("Denmark")]
	DK = 208,

	/// <summary>
	///     Dominica
	/// </summary>
	[Description("Dominica")]
	DM = 212,

	/// <summary>
	///     Dominican Republic
	/// </summary>
	[Description("Dominican Republic")]
	DO = 214,

	/// <summary>
	///     Ecuador
	/// </summary>
	[Description("Ecuador")]
	EC = 218,

	/// <summary>
	///     El Salvador
	/// </summary>
	[Description("El Salvador")]
	SV = 222,

	/// <summary>
	///     Equatorial Guinea
	/// </summary>
	[Description("Equatorial Guinea")]
	GQ = 226,

	/// <summary>
	///     Ethiopia
	/// </summary>
	[Description("Ethiopia")]
	ET = 231,

	/// <summary>
	///     Eritrea
	/// </summary>
	[Description("Eritrea")]
	ER = 232,

	/// <summary>
	///     Estonia
	/// </summary>
	[Description("Estonia")]
	EE = 233,

	/// <summary>
	///     Faroe Islands
	/// </summary>
	[Description("Faroe Islands")]
	FO = 234,

	/// <summary>
	///     Falkland Islands (Malvinas)
	/// </summary>
	[Description("Falkland Islands (Malvinas)")]
	FK = 238,

	/// <summary>
	///     South Georgia and the South Sandwich Islands
	/// </summary>
	[Description("South Georgia and the South Sandwich Islands")]
	GS = 239,

	/// <summary>
	///     Fiji
	/// </summary>
	[Description("Fiji")]
	FJ = 242,

	/// <summary>
	///     Finland
	/// </summary>
	[Description("Finland")]
	FI = 246,

	/// <summary>
	///     Åland Islands
	/// </summary>
	[Description("Åland Islands")]
	AX = 248,

	/// <summary>
	///     France
	/// </summary>
	[Description("France")]
	FR = 250,

	/// <summary>
	///     French Guiana
	/// </summary>
	[Description("French Guiana")]
	GF = 254,

	/// <summary>
	///     French Polynesia
	/// </summary>
	[Description("French Polynesia")]
	PF = 258,

	/// <summary>
	///     French Southern Territories
	/// </summary>
	[Description("French Southern Territories")]
	TF = 260,

	/// <summary>
	///     Djibouti
	/// </summary>
	[Description("Djibouti")]
	DJ = 262,

	/// <summary>
	///     Gabon
	/// </summary>
	[Description("Gabon")]
	GA = 266,

	/// <summary>
	///     Georgia
	/// </summary>
	[Description("Georgia")]
	GE = 268,

	/// <summary>
	///     Gambia
	/// </summary>
	[Description("Gambia")]
	GM = 270,

	/// <summary>
	///     Palestine, State of
	/// </summary>
	[Description("Palestine, State of")]
	PS = 275,

	/// <summary>
	///     Germany
	/// </summary>
	[Description("Germany")]
	DE = 276,

	/// <summary>
	///     Ghana
	/// </summary>
	[Description("Ghana")]
	GH = 288,

	/// <summary>
	///     Gibraltar
	/// </summary>
	[Description("Gibraltar")]
	GI = 292,

	/// <summary>
	///     Kiribati
	/// </summary>
	[Description("Kiribati")]
	KI = 296,

	/// <summary>
	///     Greece
	/// </summary>
	[Description("Greece")]
	GR = 300,

	/// <summary>
	///     Greenland
	/// </summary>
	[Description("Greenland")]
	GL = 304,

	/// <summary>
	///     Grenada
	/// </summary>
	[Description("Grenada")]
	GD = 308,

	/// <summary>
	///     Guadeloupe
	/// </summary>
	[Description("Guadeloupe")]
	GP = 312,

	/// <summary>
	///     Guam
	/// </summary>
	[Description("Guam")]
	GU = 316,

	/// <summary>
	///     Guatemala
	/// </summary>
	[Description("Guatemala")]
	GT = 320,

	/// <summary>
	///     Guinea
	/// </summary>
	[Description("Guinea")]
	GN = 324,

	/// <summary>
	///     Guyana
	/// </summary>
	[Description("Guyana")]
	GY = 328,

	/// <summary>
	///     Haiti
	/// </summary>
	[Description("Haiti")]
	HT = 332,

	/// <summary>
	///     Heard Island and McDonald Islands
	/// </summary>
	[Description("Heard Island and McDonald Islands")]
	HM = 334,

	/// <summary>
	///     Holy See
	/// </summary>
	[Description("Holy See")]
	VA = 336,

	/// <summary>
	///     Honduras
	/// </summary>
	[Description("Honduras")]
	HN = 340,

	/// <summary>
	///     Hong Kong
	/// </summary>
	[Description("Hong Kong")]
	HK = 344,

	/// <summary>
	///     Hungary
	/// </summary>
	[Description("Hungary")]
	HU = 348,

	/// <summary>
	///     Iceland
	/// </summary>
	[Description("Iceland")]
	IS = 352,

	/// <summary>
	///     India
	/// </summary>
	[Description("India")]
	IN = 356,

	/// <summary>
	///     Indonesia
	/// </summary>
	[Description("Indonesia")]
	ID = 360,

	/// <summary>
	///     Iran (Islamic Republic of)
	/// </summary>
	[Description("Iran (Islamic Republic of)")]
	IR = 364,

	/// <summary>
	///     Iraq
	/// </summary>
	[Description("Iraq")]
	IQ = 368,

	/// <summary>
	///     Ireland
	/// </summary>
	[Description("Ireland")]
	IE = 372,

	/// <summary>
	///     Israel
	/// </summary>
	[Description("Israel")]
	IL = 376,

	/// <summary>
	///     Italy
	/// </summary>
	[Description("Italy")]
	IT = 380,

	/// <summary>
	///     Côte d'Ivoire
	/// </summary>
	[Description("Côte d'Ivoire")]
	CI = 384,

	/// <summary>
	///     Jamaica
	/// </summary>
	[Description("Jamaica")]
	JM = 388,

	/// <summary>
	///     Japan
	/// </summary>
	[Description("Japan")]
	JP = 392,

	/// <summary>
	///     Kazakhstan
	/// </summary>
	[Description("Kazakhstan")]
	KZ = 398,

	/// <summary>
	///     Jordan
	/// </summary>
	[Description("Jordan")]
	JO = 400,

	/// <summary>
	///     Kenya
	/// </summary>
	[Description("Kenya")]
	KE = 404,

	/// <summary>
	///     Korea (Democratic People's Republic of)
	/// </summary>
	[Description("Korea (Democratic People's Republic of)")]
	KP = 408,

	/// <summary>
	///     Korea, Republic of
	/// </summary>
	[Description("Korea, Republic of")]
	KR = 410,

	/// <summary>
	///     Kuwait
	/// </summary>
	[Description("Kuwait")]
	KW = 414,

	/// <summary>
	///     Kyrgyzstan
	/// </summary>
	[Description("Kyrgyzstan")]
	KG = 417,

	/// <summary>
	///     Lao People's Democratic Republic
	/// </summary>
	[Description("Lao People's Democratic Republic")]
	LA = 418,

	/// <summary>
	///     Lebanon
	/// </summary>
	[Description("Lebanon")]
	LB = 422,

	/// <summary>
	///     Lesotho
	/// </summary>
	[Description("Lesotho")]
	LS = 426,

	/// <summary>
	///     Latvia
	/// </summary>
	[Description("Latvia")]
	LV = 428,

	/// <summary>
	///     Liberia
	/// </summary>
	[Description("Liberia")]
	LR = 430,

	/// <summary>
	///     Libya
	/// </summary>
	[Description("Libya")]
	LY = 434,

	/// <summary>
	///     Liechtenstein
	/// </summary>
	[Description("Liechtenstein")]
	LI = 438,

	/// <summary>
	///     Lithuania
	/// </summary>
	[Description("Lithuania")]
	LT = 440,

	/// <summary>
	///     Luxembourg
	/// </summary>
	[Description("Luxembourg")]
	LU = 442,

	/// <summary>
	///     Macao
	/// </summary>
	[Description("Macao")]
	MO = 446,

	/// <summary>
	///     Madagascar
	/// </summary>
	[Description("Madagascar")]
	MG = 450,

	/// <summary>
	///     Malawi
	/// </summary>
	[Description("Malawi")]
	MW = 454,

	/// <summary>
	///     Malaysia
	/// </summary>
	[Description("Malaysia")]
	MY = 458,

	/// <summary>
	///     Maldives
	/// </summary>
	[Description("Maldives")]
	MV = 462,

	/// <summary>
	///     Mali
	/// </summary>
	[Description("Mali")]
	ML = 466,

	/// <summary>
	///     Malta
	/// </summary>
	[Description("Malta")]
	MT = 470,

	/// <summary>
	///     Martinique
	/// </summary>
	[Description("Martinique")]
	MQ = 474,

	/// <summary>
	///     Mauritania
	/// </summary>
	[Description("Mauritania")]
	MR = 478,

	/// <summary>
	///     Mauritius
	/// </summary>
	[Description("Mauritius")]
	MU = 480,

	/// <summary>
	///     Mexico
	/// </summary>
	[Description("Mexico")]
	MX = 484,

	/// <summary>
	///     Monaco
	/// </summary>
	[Description("Monaco")]
	MC = 492,

	/// <summary>
	///     Mongolia
	/// </summary>
	[Description("Mongolia")]
	MN = 496,

	/// <summary>
	///     Moldova, Republic of
	/// </summary>
	[Description("Moldova, Republic of")]
	MD = 498,

	/// <summary>
	///     Montenegro
	/// </summary>
	[Description("Montenegro")]
	ME = 499,

	/// <summary>
	///     Montserrat
	/// </summary>
	[Description("Montserrat")]
	MS = 500,

	/// <summary>
	///     Morocco
	/// </summary>
	[Description("Morocco")]
	MA = 504,

	/// <summary>
	///     Mozambique
	/// </summary>
	[Description("Mozambique")]
	MZ = 508,

	/// <summary>
	///     Oman
	/// </summary>
	[Description("Oman")]
	OM = 512,

	/// <summary>
	///     Namibia
	/// </summary>
	[Description("Namibia")]
	NA = 516,

	/// <summary>
	///     Nauru
	/// </summary>
	[Description("Nauru")]
	NR = 520,

	/// <summary>
	///     Nepal
	/// </summary>
	[Description("Nepal")]
	NP = 524,

	/// <summary>
	///     Netherlands
	/// </summary>
	[Description("Netherlands")]
	NL = 528,

	/// <summary>
	///     Curaçao
	/// </summary>
	[Description("Curaçao")]
	CW = 531,

	/// <summary>
	///     Aruba
	/// </summary>
	[Description("Aruba")]
	AW = 533,

	/// <summary>
	///     Sint Maarten (Dutch part)
	/// </summary>
	[Description("Sint Maarten (Dutch part)")]
	SX = 534,

	/// <summary>
	///     Bonaire, Sint Eustatius and Saba
	/// </summary>
	[Description("Bonaire, Sint Eustatius and Saba")]
	BQ = 535,

	/// <summary>
	///     New Caledonia
	/// </summary>
	[Description("New Caledonia")]
	NC = 540,

	/// <summary>
	///     Vanuatu
	/// </summary>
	[Description("Vanuatu")]
	VU = 548,

	/// <summary>
	///     New Zealand
	/// </summary>
	[Description("New Zealand")]
	NZ = 554,

	/// <summary>
	///     Nicaragua
	/// </summary>
	[Description("Nicaragua")]
	NI = 558,

	/// <summary>
	///     Niger
	/// </summary>
	[Description("Niger")]
	NE = 562,

	/// <summary>
	///     Nigeria
	/// </summary>
	[Description("Nigeria")]
	NG = 566,

	/// <summary>
	///     Niue
	/// </summary>
	[Description("Niue")]
	NU = 570,

	/// <summary>
	///     Norfolk Island
	/// </summary>
	[Description("Norfolk Island")]
	NF = 574,

	/// <summary>
	///     Norway
	/// </summary>
	[Description("Norway")]
	NO = 578,

	/// <summary>
	///     Northern Mariana Islands
	/// </summary>
	[Description("Northern Mariana Islands")]
	MP = 580,

	/// <summary>
	///     United States Minor Outlying Islands
	/// </summary>
	[Description("United States Minor Outlying Islands")]
	UM = 581,

	/// <summary>
	///     Micronesia (Federated States of)
	/// </summary>
	[Description("Micronesia (Federated States of)")]
	FM = 583,

	/// <summary>
	///     Marshall Islands
	/// </summary>
	[Description("Marshall Islands")]
	MH = 584,

	/// <summary>
	///     Palau
	/// </summary>
	[Description("Palau")]
	PW = 585,

	/// <summary>
	///     Pakistan
	/// </summary>
	[Description("Pakistan")]
	PK = 586,

	/// <summary>
	///     Panama
	/// </summary>
	[Description("Panama")]
	PA = 591,

	/// <summary>
	///     Papua New Guinea
	/// </summary>
	[Description("Papua New Guinea")]
	PG = 598,

	/// <summary>
	///     Paraguay
	/// </summary>
	[Description("Paraguay")]
	PY = 600,

	/// <summary>
	///     Peru
	/// </summary>
	[Description("Peru")]
	PE = 604,

	/// <summary>
	///     Philippines
	/// </summary>
	[Description("Philippines")]
	PH = 608,

	/// <summary>
	///     Pitcairn
	/// </summary>
	[Description("Pitcairn")]
	PN = 612,

	/// <summary>
	///     Poland
	/// </summary>
	[Description("Poland")]
	PL = 616,

	/// <summary>
	///     Portugal
	/// </summary>
	[Description("Portugal")]
	PT = 620,

	/// <summary>
	///     Guinea-Bissau
	/// </summary>
	[Description("Guinea-Bissau")]
	GW = 624,

	/// <summary>
	///     Timor-Leste
	/// </summary>
	[Description("Timor-Leste")]
	TL = 626,

	/// <summary>
	///     Puerto Rico
	/// </summary>
	[Description("Puerto Rico")]
	PR = 630,

	/// <summary>
	///     Qatar
	/// </summary>
	[Description("Qatar")]
	QA = 634,

	/// <summary>
	///     Réunion
	/// </summary>
	[Description("Réunion")]
	RE = 638,

	/// <summary>
	///     Romania
	/// </summary>
	[Description("Romania")]
	RO = 642,

	/// <summary>
	///     Russian Federation
	/// </summary>
	[Description("Russian Federation")]
	RU = 643,

	/// <summary>
	///     Rwanda
	/// </summary>
	[Description("Rwanda")]
	RW = 646,

	/// <summary>
	///     Saint Barthélemy
	/// </summary>
	[Description("Saint Barthélemy")]
	BL = 652,

	/// <summary>
	///     Saint Helena, Ascension and Tristan da Cunha
	/// </summary>
	[Description("Saint Helena, Ascension and Tristan da Cunha")]
	SH = 654,

	/// <summary>
	///     Saint Kitts and Nevis
	/// </summary>
	[Description("Saint Kitts and Nevis")]
	KN = 659,

	/// <summary>
	///     Anguilla
	/// </summary>
	[Description("Anguilla")]
	AI = 660,

	/// <summary>
	///     Saint Lucia
	/// </summary>
	[Description("Saint Lucia")]
	LC = 662,

	/// <summary>
	///     Saint Martin (French part)
	/// </summary>
	[Description("Saint Martin (French part)")]
	MF = 663,

	/// <summary>
	///     Saint Pierre and Miquelon
	/// </summary>
	[Description("Saint Pierre and Miquelon")]
	PM = 666,

	/// <summary>
	///     Saint Vincent and the Grenadines
	/// </summary>
	[Description("Saint Vincent and the Grenadines")]
	VC = 670,

	/// <summary>
	///     San Marino
	/// </summary>
	[Description("San Marino")]
	SM = 674,

	/// <summary>
	///     Sao Tome and Principe
	/// </summary>
	[Description("Sao Tome and Principe")]
	ST = 678,

	/// <summary>
	///     Saudi Arabia
	/// </summary>
	[Description("Saudi Arabia")]
	SA = 682,

	/// <summary>
	///     Senegal
	/// </summary>
	[Description("Senegal")]
	SN = 686,

	/// <summary>
	///     Serbia
	/// </summary>
	[Description("Serbia")]
	RS = 688,

	/// <summary>
	///     Seychelles
	/// </summary>
	[Description("Seychelles")]
	SC = 690,

	/// <summary>
	///     Sierra Leone
	/// </summary>
	[Description("Sierra Leone")]
	SL = 694,

	/// <summary>
	///     Singapore
	/// </summary>
	[Description("Singapore")]
	SG = 702,

	/// <summary>
	///     Slovakia
	/// </summary>
	[Description("Slovakia")]
	SK = 703,

	/// <summary>
	///     Viet Nam
	/// </summary>
	[Description("Viet Nam")]
	VN = 704,

	/// <summary>
	///     Slovenia
	/// </summary>
	[Description("Slovenia")]
	SI = 705,

	/// <summary>
	///     Somalia
	/// </summary>
	[Description("Somalia")]
	SO = 706,

	/// <summary>
	///     South Africa
	/// </summary>
	[Description("South Africa")]
	ZA = 710,

	/// <summary>
	///     Zimbabwe
	/// </summary>
	[Description("Zimbabwe")]
	ZW = 716,

	/// <summary>
	///     Spain
	/// </summary>
	[Description("Spain")]
	ES = 724,

	/// <summary>
	///     South Sudan
	/// </summary>
	[Description("South Sudan")]
	SS = 728,

	/// <summary>
	///     Sudan
	/// </summary>
	[Description("Sudan")]
	SD = 729,

	/// <summary>
	///     Western Sahara
	/// </summary>
	[Description("Western Sahara")]
	EH = 732,

	/// <summary>
	///     Suriname
	/// </summary>
	[Description("Suriname")]
	SR = 740,

	/// <summary>
	///     Svalbard and Jan Mayen
	/// </summary>
	[Description("Svalbard and Jan Mayen")]
	SJ = 744,

	/// <summary>
	///     Eswatini
	/// </summary>
	[Description("Eswatini")]
	SZ = 748,

	/// <summary>
	///     Sweden
	/// </summary>
	[Description("Sweden")]
	SE = 752,

	/// <summary>
	///     Switzerland
	/// </summary>
	[Description("Switzerland")]
	CH = 756,

	/// <summary>
	///     Syrian Arab Republic
	/// </summary>
	[Description("Syrian Arab Republic")]
	SY = 760,

	/// <summary>
	///     Tajikistan
	/// </summary>
	[Description("Tajikistan")]
	TJ = 762,

	/// <summary>
	///     Thailand
	/// </summary>
	[Description("Thailand")]
	TH = 764,

	/// <summary>
	///     Togo
	/// </summary>
	[Description("Togo")]
	TG = 768,

	/// <summary>
	///     Tokelau
	/// </summary>
	[Description("Tokelau")]
	TK = 772,

	/// <summary>
	///     Tonga
	/// </summary>
	[Description("Tonga")]
	TO = 776,

	/// <summary>
	///     Trinidad and Tobago
	/// </summary>
	[Description("Trinidad and Tobago")]
	TT = 780,

	/// <summary>
	///     United Arab Emirates
	/// </summary>
	[Description("United Arab Emirates")]
	AE = 784,

	/// <summary>
	///     Tunisia
	/// </summary>
	[Description("Tunisia")]
	TN = 788,

	/// <summary>
	///     Turkey
	/// </summary>
	[Description("Turkey")]
	TR = 792,

	/// <summary>
	///     Turkmenistan
	/// </summary>
	[Description("Turkmenistan")]
	TM = 795,

	/// <summary>
	///     Turks and Caicos Islands
	/// </summary>
	[Description("Turks and Caicos Islands")]
	TC = 796,

	/// <summary>
	///     Tuvalu
	/// </summary>
	[Description("Tuvalu")]
	TV = 798,

	/// <summary>
	///     Uganda
	/// </summary>
	[Description("Uganda")]
	UG = 800,

	/// <summary>
	///     Ukraine
	/// </summary>
	[Description("Ukraine")]
	UA = 804,

	/// <summary>
	///     North Macedonia
	/// </summary>
	[Description("North Macedonia")]
	MK = 807,

	/// <summary>
	///     Egypt
	/// </summary>
	[Description("Egypt")]
	EG = 818,

	/// <summary>
	///     United Kingdom of Great Britain and Northern Ireland
	/// </summary>
	[Description("United Kingdom of Great Britain and Northern Ireland")]
	GB = 826,

	/// <summary>
	///     Guernsey
	/// </summary>
	[Description("Guernsey")]
	GG = 831,

	/// <summary>
	///     Jersey
	/// </summary>
	[Description("Jersey")]
	JE = 832,

	/// <summary>
	///     Isle of Man
	/// </summary>
	[Description("Isle of Man")]
	IM = 833,

	/// <summary>
	///     Tanzania, United Republic of
	/// </summary>
	[Description("Tanzania, United Republic of")]
	TZ = 834,

	/// <summary>
	///     United States of America
	/// </summary>
	[Description("United States of America")]
	US = 840,

	/// <summary>
	///     Virgin Islands (U.S.)
	/// </summary>
	[Description("Virgin Islands (U.S.)")]
	VI = 850,

	/// <summary>
	///     Burkina Faso
	/// </summary>
	[Description("Burkina Faso")]
	BF = 854,

	/// <summary>
	///     Uruguay
	/// </summary>
	[Description("Uruguay")]
	UY = 858,

	/// <summary>
	///     Uzbekistan
	/// </summary>
	[Description("Uzbekistan")]
	UZ = 860,

	/// <summary>
	///     Venezuela (Bolivarian Republic of)
	/// </summary>
	[Description("Venezuela (Bolivarian Republic of)")]
	VE = 862,

	/// <summary>
	///     Wallis and Futuna
	/// </summary>
	[Description("Wallis and Futuna")]
	WF = 876,

	/// <summary>
	///     Samoa
	/// </summary>
	[Description("Samoa")]
	WS = 882,

	/// <summary>
	///     Yemen
	/// </summary>
	[Description("Yemen")]
	YE = 887,

	/// <summary>
	///     Zambia
	/// </summary>
	[Description("Zambia")]
	ZM = 894
}
