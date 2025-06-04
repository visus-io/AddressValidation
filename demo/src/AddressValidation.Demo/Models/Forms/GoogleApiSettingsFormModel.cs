namespace AddressValidation.Demo.Models.Forms;

public sealed class GoogleApiSettingsFormModel
{
    private string? _privateKey;

    public string? PrivateKey
    {
        get => _privateKey;
        set
        {
            if ( string.IsNullOrWhiteSpace(value) )
            {
                _privateKey = null;
                return;
            }

            if ( value.Contains("\\n", StringComparison.OrdinalIgnoreCase) )
            {
                value = value.Replace("\\n", Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            }

            _privateKey = value;
        }
    }

    public string? ProjectId { get; set; }

    public string? ServiceAccountEmailAddress { get; set; }
}
