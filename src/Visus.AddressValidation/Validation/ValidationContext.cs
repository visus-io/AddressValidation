namespace Visus.AddressValidation.Validation;

internal sealed class ValidationContext<T>(T instance)
    where T : class
{
    public readonly HashSet<ValidationState> ValidationResults = [];

    public T Instance { get; } = instance;
}
