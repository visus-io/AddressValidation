namespace Visus.AddressValidation.Validation;

/// <summary>
///     Centralized validation message templates for consistent error and warning messaging.
/// </summary>
public static class ValidationMessages
{
    // Common field validation messages
    public const string ValueCannotBeNull = "Value cannot be null.";
    public const string ValueCannotBeNullOrEmpty = "Value cannot be null or empty.";
    public const string CannotBeNullOrEmpty = "Cannot be null or empty.";

    // Address-specific validation messages
    public const string AddressLinesCannotExceedThree = "Cannot contain more than 3 entries.";
    public const string CountryNotSupportedForAddressValidation = "{0}: {1} is currently not supported for address validation.";

    // Provider-specific validation messages
    public const string CountryNotSupportedByProvider = "{0}: {1} is currently not supported by the {2} Address Validation API.";
    public const string OnlyValueSupportedInDevelopmentMode = "{0}: Only the value {1} is supported by the {2} Address Validation API while in {3} mode.";
    public const string OnlyValuesSupportedInDevelopmentMode = "{0}: Only the values {1} are supported by the {2} Address Validation API while in {3} mode.";

    // Verification result messages
    public const string ValueCouldNotBeVerified = "Value could not be verified.";
    public const string RowValueCouldNotBeVerified = "[Row {0}] {1}: {2}";

    // API response messages (formatted with error codes)
    public const string ApiErrorWithCode = "{0}: {1}";
}
