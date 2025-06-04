namespace Visus.AddressValidation.Validation;

internal sealed class ValidationResult : IValidationResult
{
    public ValidationResult(IReadOnlySet<ValidationState> states)
    {
        ArgumentNullException.ThrowIfNull(states);

        Errors = states.Where(w => w.Severity == ValidationSeverity.Error).ToHashSet();
        Warnings = states.Where(w => w.Severity == ValidationSeverity.Warning).ToHashSet();
    }

    public IReadOnlySet<ValidationState> Errors { get; }

    public bool HasErrors => Errors.Count > 0;

    public bool HasWarnings => Warnings.Count > 0;

    public IReadOnlySet<ValidationState> Warnings { get; }
}
