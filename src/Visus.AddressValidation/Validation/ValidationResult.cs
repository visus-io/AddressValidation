namespace Visus.AddressValidation.Validation;

using System.Collections.Frozen;

internal sealed class ValidationResult : IValidationResult
{
    public ValidationResult(IReadOnlySet<ValidationState> states)
    {
        ArgumentNullException.ThrowIfNull(states);

        HashSet<ValidationState> errors = [];
        HashSet<ValidationState> warnings = [];

        foreach ( ValidationState state in states )
        {
            switch ( state.Severity )
            {
                case ValidationSeverity.Error:
                    errors.Add(state);
                    break;
                case ValidationSeverity.Warning:
                    warnings.Add(state);
                    break;
            }
        }

        Errors = errors.ToFrozenSet();
        Warnings = warnings.ToFrozenSet();
    }

    public IReadOnlySet<ValidationState> Errors { get; }

    public bool HasErrors => Errors.Count > 0;

    public bool HasWarnings => Warnings.Count > 0;

    public IReadOnlySet<ValidationState> Warnings { get; }
}
