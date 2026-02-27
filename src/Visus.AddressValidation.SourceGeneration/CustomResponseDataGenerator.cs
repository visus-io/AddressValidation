namespace Visus.AddressValidation.SourceGeneration;

using System.Collections.Immutable;
using System.Text;
using Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Model;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

[Generator]
public sealed class CustomResponseDataGenerator : IIncrementalGenerator
{
    private const string CustomResponseDataPropertyAttribute = "Visus.AddressValidation.CustomResponseDataPropertyAttribute";

    private const string GetCustomResponseDataMethodName = "GetCustomResponseData";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(HierarchyInfo, PropertyInfo)> propertyInfo =
            context.SyntaxProvider
                   .ForAttributeWithMetadataName(CustomResponseDataPropertyAttribute,
                        static (node, _) => node is PropertyDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0, },
                        static (ctx, _) =>
                            ctx.TargetSymbol is not IPropertySymbol propertySymbol
                                ? default
                                : ( HierarchyInfo.From(propertySymbol.ContainingType), new PropertyInfo(propertySymbol) ));

        IncrementalValuesProvider<(HierarchyInfo Hierarchy, ImmutableArray<PropertyInfo> Properties)> groupedPropertyInfo =
            propertyInfo.GroupBy(static g => g.Left, static g => g.Right);

        context.RegisterSourceOutput(groupedPropertyInfo,
            static (ctx, item) =>
            {
                MethodDeclarationSyntax methodDeclaration =
                    MethodDeclaration(GetDictionaryMethodSyntax(), GetCustomResponseDataMethodName)
                       .AddModifiers(Token(SyntaxKind.PublicKeyword))
                       .NormalizeWhitespace()
                       .WithTrailingTrivia(CarriageReturnLineFeed)
                       .WithBody(Block(SingletonList<StatementSyntax>(GetDictionaryMethodBodySyntax(item.Properties))));

                CompilationUnitSyntax? compilationUnit = item.Hierarchy.GetCompilationUnit([methodDeclaration,]);
                if ( compilationUnit is not null )
                {
                    ctx.AddSource($"{item.Hierarchy.FileNameHint}.g", compilationUnit.GetText(Encoding.UTF8));
                }
            });
    }

    private static ReturnStatementSyntax GetDictionaryMethodBodySyntax(ImmutableArray<PropertyInfo> properties)
    {
        SyntaxNodeOrToken[] typeArguments =
        [
            PredefinedType(Token(SyntaxKind.StringKeyword)),
            Token(SyntaxKind.CommaToken),
            NullableType(PredefinedType(Token(SyntaxKind.ObjectKeyword))),
        ];

        GenericNameSyntax dictionaryType = GenericName(Identifier("Dictionary"),
            TypeArgumentList(SeparatedList<TypeSyntax>(typeArguments)));

        ArgumentListSyntax dictionaryArguments =
            ArgumentList()
               .AddArguments(Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(StringComparer)),
                    IdentifierName(nameof(StringComparer.OrdinalIgnoreCase)))));

        LinkedList<AssignmentExpressionSyntax> entities = [];

        foreach ( PropertyInfo property in properties )
        {
            BracketedArgumentListSyntax key =
                BracketedArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                    Literal(property.PropertyNameKey)))));
            AssignmentExpressionSyntax entity =
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                    ImplicitElementAccess().WithArgumentList(key),
                    IdentifierName(property.PropertyName));

            entities.AddLast(entity);
        }

        ObjectCreationExpressionSyntax dictionary =
            ObjectCreationExpression(dictionaryType, dictionaryArguments, null)
               .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                    SeparatedList<ExpressionSyntax>(entities)));

        return ReturnStatement(dictionary);
    }

    private static GenericNameSyntax GetDictionaryMethodSyntax()
    {
        SyntaxNodeOrToken[] arguments =
        [
            PredefinedType(Token(SyntaxKind.StringKeyword)),
            Token(SyntaxKind.CommaToken),
            NullableType(PredefinedType(Token(SyntaxKind.ObjectKeyword))),
        ];

        return GenericName(Identifier("IReadOnlyDictionary"),
            TypeArgumentList(SeparatedList<TypeSyntax>(arguments)));
    }
}
