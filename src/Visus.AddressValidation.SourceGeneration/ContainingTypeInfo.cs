namespace Visus.AddressValidation.SourceGeneration;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

/// <summary>
///     Holds metadata about a type that contains another type, used during source generation.
/// </summary>
/// <param name="Name">The simple (unqualified) name of the containing type.</param>
/// <param name="FullName">The fully qualified name of the containing type.</param>
/// <param name="Namespace">The namespace in which the containing type is declared.</param>
/// <param name="Accessibility">The declared accessibility of the containing type.</param>
/// <param name="IsRecord">
///     <see langword="true" /> if the containing type is a <c>record</c>; otherwise,
///     <see langword="false" />.
/// </param>
/// <param name="IsSealed">
///     <see langword="true" /> if the containing type is <see langword="sealed" />; otherwise,
///     <see langword="false" />.
/// </param>
/// <param name="ContainingTypes">
///     The ordered list of types that further enclose this type, from outermost to innermost.
/// </param>
internal sealed record ContainingTypeInfo(
    string Name,
    string FullName,
    string Namespace,
    Accessibility Accessibility,
    bool IsRecord,
    bool IsSealed,
    ImmutableArray<ContainingTypeInfo> ContainingTypes);
