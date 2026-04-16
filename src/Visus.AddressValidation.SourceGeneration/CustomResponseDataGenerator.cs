namespace Visus.AddressValidation.SourceGeneration;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <summary>
///     An incremental source generator that produces <c>GetCustomResponseData</c> method
///     implementations for types whose properties are annotated with
///     <c>CustomResponseDataPropertyAttribute</c>.
/// </summary>
/// <remarks>
///     For every type that contains at least one property marked with
///     <c>CustomResponseDataPropertyAttribute</c>, the generator emits a partial class/record
///     declaration that implements a method returning an
///     <see cref="IReadOnlyDictionary{TKey,TValue}">IReadOnlyDictionary&lt;string, object?&gt;</see>
///     mapping each property's key to its current value.
/// </remarks>
[Generator]
public sealed class CustomResponseDataGenerator : IIncrementalGenerator
{
    private const string CustomResponseDataPropertyAttribute = "Visus.AddressValidation.CustomResponseDataPropertyAttribute";

    private const string GetCustomResponseDataMethodName = "GetCustomResponseData";

    /// <summary>
    ///     Registers the incremental generation pipeline.
    /// </summary>
    /// <param name="context">
    ///     The <see cref="IncrementalGeneratorInitializationContext" /> used to register syntax
    ///     providers and source outputs.
    /// </param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<PropertyInfo?> propertyProvider =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                CustomResponseDataPropertyAttribute,
                static (node, _) => node is PropertyDeclarationSyntax,
                static (ctx, ct) => Transform(ctx, ct)
            );

        IncrementalValueProvider<ImmutableArray<PropertyInfo?>> collected =
            propertyProvider.Where(static w => w is not null)
                            .Collect();

        context.RegisterSourceOutput(
            collected,
            static (spc, properties) => GenerateSource(spc, properties));
    }

    private static ContainingTypeInfo BuildContainingTypeInfo(INamedTypeSymbol type)
    {
        ImmutableArray<ContainingTypeInfo> parents = type.ContainingType is not null
                                                         ? [BuildContainingTypeInfo(type.ContainingType),]
                                                         : [];

        return new ContainingTypeInfo(
            type.Name,
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            type.ContainingNamespace.ToDisplayString(),
            type.DeclaredAccessibility,
            type.IsRecord,
            type.IsSealed,
            parents);
    }

    private static SyntaxNodeOrToken[] CreateStringObjectTypeArguments()
    {
        return
        [
            PredefinedType(Token(SyntaxKind.StringKeyword)),
            Token(SyntaxKind.CommaToken),
            NullableType(PredefinedType(Token(SyntaxKind.ObjectKeyword))),
        ];
    }

    private static TypeDeclarationSyntax CreateTypeDeclaration(ContainingTypeInfo info)
    {
        SyntaxTokenList modifiers = TokenList(SyntaxGenerationHelpers.GetAccessibilityTokens(info.Accessibility));

        if ( info.IsSealed )
        {
            modifiers = modifiers.Add(Token(SyntaxKind.SealedKeyword));
        }

        modifiers = modifiers.Add(Token(SyntaxKind.PartialKeyword));

        if ( info.IsRecord )
        {
            return RecordDeclaration(Token(SyntaxKind.RecordKeyword), info.Name)
                  .WithModifiers(modifiers)
                  .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                  .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
        }

        return ClassDeclaration(info.Name).WithModifiers(modifiers);
    }

    private static void GenerateSource(SourceProductionContext context, ImmutableArray<PropertyInfo?> properties)
    {
        if ( properties.IsDefaultOrEmpty )
        {
            return;
        }

        IEnumerable<IGrouping<string, PropertyInfo>> groups =
            properties.OfType<PropertyInfo>()
                      .GroupBy(g => g.ContainingType.FullName, StringComparer.OrdinalIgnoreCase);

        foreach ( IGrouping<string, PropertyInfo>? group in groups )
        {
            string safeKey = StringHelpers.ToSafeFileName(group.Key);

            PropertyInfo first = group.First();
            IReadOnlyList<ContainingTypeInfo> typeChain = GetTypeChain(first.ContainingType);

            MethodDeclarationSyntax methodDeclaration =
                MethodDeclaration(GetDictionaryMethodSyntax(), GetCustomResponseDataMethodName)
                   .AddModifiers(Token(SyntaxKind.PublicKeyword))
                   .NormalizeWhitespace()
                   .WithTrailingTrivia(CarriageReturnLineFeed)
                   .WithBody(Block(SingletonList<StatementSyntax>(GetDictionaryMethodBodySyntax(group))));

            TypeDeclarationSyntax typeDeclaration =
                CreateTypeDeclaration(typeChain[^1])
                   .AddMembers(methodDeclaration);

            for ( int i = typeChain.Count - 2; i >= 0; i-- )
            {
                typeDeclaration = CreateTypeDeclaration(typeChain[i])
                   .AddMembers(typeDeclaration);
            }

            FileScopedNamespaceDeclarationSyntax namespaceDeclaration =
                FileScopedNamespaceDeclaration(IdentifierName(first.ContainingType.Namespace))
                   .AddMembers(typeDeclaration);

            CompilationUnitSyntax compilationUnit =
                CompilationUnit()
                   .AddUsings(UsingDirective(IdentifierName("System.Collections.Generic")))
                   .AddMembers(namespaceDeclaration)
                   .NormalizeWhitespace();

            compilationUnit = SyntaxGenerationHelpers.AddAutoGeneratedHeader(compilationUnit);

            context.AddSource($"{safeKey}_{GetCustomResponseDataMethodName}.g.cs", compilationUnit.ToFullString());
        }
    }

    private static ReturnStatementSyntax GetDictionaryMethodBodySyntax(IEnumerable<PropertyInfo> properties)
    {
        GenericNameSyntax dictionaryType = GenericName(Identifier("Dictionary"),
            TypeArgumentList(SeparatedList<TypeSyntax>(CreateStringObjectTypeArguments())));

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
                    Literal(property.PropertyKey)))));

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
        return GenericName(Identifier("IReadOnlyDictionary"),
            TypeArgumentList(SeparatedList<TypeSyntax>(CreateStringObjectTypeArguments())));
    }

    private static IReadOnlyList<ContainingTypeInfo> GetTypeChain(ContainingTypeInfo info)
    {
        List<ContainingTypeInfo> chain = [];

        chain.AddRange(info.ContainingTypes.SelectMany(GetTypeChain));
        chain.Add(info);

        return chain;
    }

    private static PropertyInfo? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ( context.TargetSymbol is not IPropertySymbol property )
        {
            return null;
        }

        INamedTypeSymbol? containingType = property.ContainingType;
        if ( containingType is null )
        {
            return null;
        }

        AttributeData attributeData = context.Attributes[0];
        string propertyKey;

        if ( attributeData.ConstructorArguments.Length > 0
          && attributeData.ConstructorArguments[0].Value is string s
          && !string.IsNullOrWhiteSpace(s) )
        {
            propertyKey = StringHelpers.ToCamelCase(s);
        }
        else
        {
            propertyKey = StringHelpers.ToCamelCase(property.Name);
        }

        return new PropertyInfo(
            BuildContainingTypeInfo(containingType),
            propertyKey,
            property.Name,
            property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        );
    }
}
