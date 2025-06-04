// This file is ported and adapted from CommunityToolkit.Mvvm (CommunityToolkit/dotnet),
// more info in ThirdPartyNotices.txt in the root of the project.

namespace Visus.AddressValidation.SourceGeneration.Model;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal sealed record TypeInfo(
    string QualifiedName,
    TypeKind Kind,
    Accessibility DeclaredAccessibility,
    bool IsRecord,
    bool IsSealed)
{
    public SyntaxKind AccessibilityKind =>
        DeclaredAccessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            _ => SyntaxKind.None
        };

    public Accessibility DeclaredAccessibility { get; } = DeclaredAccessibility;

    public bool IsRecord { get; } = IsRecord;

    public bool IsSealed { get; } = IsSealed;

    public TypeKind Kind { get; } = Kind;

    public string QualifiedName { get; } = QualifiedName;

    public TypeDeclarationSyntax GetSyntax()
    {
        return Kind switch
        {
            TypeKind.Class when IsRecord =>
                RecordDeclaration(Token(SyntaxKind.RecordKeyword), QualifiedName)
                   .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                   .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken)),
            _ => ClassDeclaration(QualifiedName)
        };
    }
}
