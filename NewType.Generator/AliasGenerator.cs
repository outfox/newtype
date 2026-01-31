using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace newtype.generator;

/// <summary>
/// Incremental source generator that creates type alias implementations.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AliasGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(ctx => { ctx.AddSource("newtypeAttribute.g.cs", SourceText.From(NewtypeAttributeSource.Source, Encoding.UTF8)); });

        // Find all struct declarations with our attribute
        var aliasDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateStruct(node),
                transform: static (ctx, _) => GetAliasInfo(ctx))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!.Value);

        // Combine with compilation
        var compilationAndAliases = context.CompilationProvider.Combine(aliasDeclarations.Collect());

        // Generate the source
        context.RegisterSourceOutput(compilationAndAliases, static (spc, source) =>
        {
            var (compilation, aliases) = source;
            foreach (var alias in aliases)
            {
                GenerateAliasCode(spc, compilation, alias);
            }
        });
    }

    private static bool IsCandidateStruct(SyntaxNode node)
    {
        if (node is StructDeclarationSyntax {AttributeLists.Count: > 0} structDecl)
            return structDecl.Modifiers.Any(SyntaxKind.PartialKeyword);

        if (node is RecordDeclarationSyntax {AttributeLists.Count: > 0} recordDecl
            && recordDecl.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
            return recordDecl.Modifiers.Any(SyntaxKind.PartialKeyword);

        return false;
    }

    private static AliasInfo? GetAliasInfo(GeneratorSyntaxContext context)
    {
        var structDecl = (TypeDeclarationSyntax) context.Node;
        var semanticModel = context.SemanticModel;

        foreach (var attributeList in structDecl.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(attribute);
                if (symbolInfo.Symbol is not IMethodSymbol attributeConstructor)
                    continue;

                var attributeType = attributeConstructor.ContainingType;
                var fullName = attributeType.ToDisplayString();

                // Check for generic Alias<T>
                if (attributeType.IsGenericType &&
                    attributeType.OriginalDefinition.ToDisplayString() == "newtype.newtypeAttribute<T>")
                {
                    var aliasedType = attributeType.TypeArguments[0];
                    var structSymbol = semanticModel.GetDeclaredSymbol(structDecl);
                    if (structSymbol is null) continue;

                    return new AliasInfo(
                        structDecl,
                        structSymbol,
                        aliasedType);
                }

                // Check for non-generic Alias(typeof(T))
                if (fullName == "newtype.newtypeAttribute")
                {
                    var attributeData = semanticModel.GetDeclaredSymbol(structDecl)?
                        .GetAttributes()
                        .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "newtype.newtypeAttribute");

                    if (attributeData?.ConstructorArguments.Length > 0 &&
                        attributeData.ConstructorArguments[0].Value is ITypeSymbol aliasedType)
                    {
                        var structSymbol = semanticModel.GetDeclaredSymbol(structDecl);
                        if (structSymbol is null) continue;

                        return new AliasInfo(
                            structDecl,
                            structSymbol,
                            aliasedType);
                    }
                }
            }
        }

        return null;
    }

    private static void GenerateAliasCode(
        SourceProductionContext context,
        Compilation compilation,
        AliasInfo alias)
    {
        var generator = new AliasCodeGenerator(compilation, alias);
        var source = generator.Generate();

        var fileName = $"{alias.StructSymbol.ToDisplayString().Replace(".", "_").Replace("<", "_").Replace(">", "_")}.g.cs";
        context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
    }
}

internal readonly record struct AliasInfo(
    TypeDeclarationSyntax StructDeclaration,
    INamedTypeSymbol StructSymbol,
    ITypeSymbol AliasedType);