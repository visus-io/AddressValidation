namespace Visus.AddressValidation.SourceGeneration.Model;

using Microsoft.CodeAnalysis;

internal readonly struct PropertyInfo : IEquatable<PropertyInfo>
{
    public PropertyInfo(IPropertySymbol propertySymbol)
    {
        if ( propertySymbol is null )
        {
            throw new ArgumentNullException(nameof(propertySymbol));
        }

        AttributeData attribute = propertySymbol.GetAttributes()[0];

        PropertyName = propertySymbol.Name;

        if ( attribute.ConstructorArguments.Length == 1 )
        {
            string? attributeValue = attribute.ConstructorArguments[0].Value!.ToString();
            PropertyNameKey = char.ToLowerInvariant(attributeValue[0]) + attributeValue.Substring(1);
        }
        else
        {
            PropertyNameKey = char.ToLowerInvariant(propertySymbol.Name[0]) + propertySymbol.Name.Substring(1);
        }
    }

    public string PropertyName { get; }

    public string PropertyNameKey { get; }

    public static bool operator ==(PropertyInfo left, PropertyInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PropertyInfo left, PropertyInfo right)
    {
        return !left.Equals(right);
    }

    public bool Equals(PropertyInfo other)
    {
        return string.Equals(PropertyName, other.PropertyName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(PropertyNameKey, other.PropertyNameKey, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is PropertyInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(PropertyName);

            hashCode = ( hashCode * 397 ) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(PropertyNameKey);

            return hashCode;
        }
    }
}
