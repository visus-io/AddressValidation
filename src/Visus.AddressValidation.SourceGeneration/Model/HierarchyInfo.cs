// This file is ported and adapted from CommunityToolkit.Mvvm (CommunityToolkit/dotnet),
// more info in ThirdPartyNotices.txt in the root of the project.

namespace Visus.AddressValidation.SourceGeneration.Model;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

internal sealed partial class HierarchyInfo : IEquatable<HierarchyInfo>
{
    private HierarchyInfo(string fileNameHint, string @namespace, ImmutableArray<TypeInfo> hierarchy)
    {
        if ( string.IsNullOrWhiteSpace(fileNameHint) )
        {
            throw new ArgumentException($"'{nameof(fileNameHint)}' cannot be null or empty.", nameof(fileNameHint));
        }

        FileNameHint = fileNameHint;
        Hierarchy = hierarchy;
        Namespace = @namespace;
    }

    public string FileNameHint { get; }

    public ImmutableArray<TypeInfo> Hierarchy { get; }

    public string Namespace { get; }

    public static HierarchyInfo From(INamedTypeSymbol typeSymbol)
    {
        if ( typeSymbol is null )
        {
            throw new ArgumentNullException(nameof(typeSymbol));
        }

        LinkedList<TypeInfo> hierarchy = [];

        for ( INamedTypeSymbol? parent = typeSymbol;
             parent is not null;
             parent = parent.ContainingType )
        {
            hierarchy.AddLast(new TypeInfo(parent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                parent.TypeKind,
                parent.DeclaredAccessibility,
                parent.IsRecord,
                parent.IsSealed));
        }

        return new HierarchyInfo(typeSymbol.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: NameAndContainingTypesAndNamespaces)),
            typeSymbol.ContainingNamespace.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: NameAndContainingTypesAndNamespaces)),
            [..hierarchy,]);
    }

    public static bool operator ==(HierarchyInfo? left, HierarchyInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(HierarchyInfo? left, HierarchyInfo? right)
    {
        return !Equals(left, right);
    }

    public bool Equals(HierarchyInfo? other)
    {
        if ( other is null )
        {
            return false;
        }

        if ( ReferenceEquals(this, other) )
        {
            return true;
        }

        return string.Equals(FileNameHint, other.FileNameHint, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || ( obj is HierarchyInfo other && Equals(other) );
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ( StringComparer.OrdinalIgnoreCase.GetHashCode(FileNameHint) * 397 )
                 ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace);
        }
    }
}
