using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace newtype.generator;

/// <summary>
/// Incremental source generator that creates type alias implementations.
/// Uses ForAttributeWithMetadataName for efficient attribute-based incremental generation.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AliasGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(ctx => { ctx.AddSource("newtypeAttribute.g.cs", SourceText.From(NewtypeAttributeSource.Source, Encoding.UTF8)); });

        // Pipeline for generic [newtype<T>] attribute
        var genericPipeline = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "newtype.newtypeAttribute`1",
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: static (ctx, _) => ExtractGenericModel(ctx))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

        context.RegisterSourceOutput(genericPipeline, static (spc, model) => GenerateAliasCode(spc, model));

        // Pipeline for non-generic [newtype(typeof(T))] attribute
        var nonGenericPipeline = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "newtype.newtypeAttribute",
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: static (ctx, _) => ExtractNonGenericModel(ctx))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

        context.RegisterSourceOutput(nonGenericPipeline, static (spc, model) => GenerateAliasCode(spc, model));
    }

    private static AliasModel? ExtractGenericModel(GeneratorAttributeSyntaxContext context)
    {
        foreach (var attributeData in context.Attributes)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass is {IsGenericType: true} &&
                attributeClass.TypeArguments.Length == 1)
            {
                var aliasedType = attributeClass.TypeArguments[0];
                var (options, methodImpl) = ExtractNamedArguments(attributeData);
                return AliasModelExtractor.Extract(context, aliasedType, options, methodImpl);
            }
        }
        return null;
    }

    private static AliasModel? ExtractNonGenericModel(GeneratorAttributeSyntaxContext context)
    {
        foreach (var attributeData in context.Attributes)
        {
            if (attributeData.ConstructorArguments.Length > 0 &&
                attributeData.ConstructorArguments[0].Value is ITypeSymbol aliasedType)
            {
                var (options, methodImpl) = ExtractNamedArguments(attributeData);
                return AliasModelExtractor.Extract(context, aliasedType, options, methodImpl);
            }
        }
        return null;
    }

    // Mirrors MethodImplOptions.AggressiveInlining — the generator can't reference
    // the injected NewtypeOptions enum, and we want readable defaults.
    private const int DefaultOptions = 0;
    private const int DefaultMethodImplAggressiveInlining = 256;

    private static (int options, int methodImpl) ExtractNamedArguments(AttributeData attributeData)
    {
        int options = DefaultOptions;
        int methodImpl = DefaultMethodImplAggressiveInlining;

        foreach (var arg in attributeData.NamedArguments)
        {
            switch (arg.Key)
            {
                case "Options":
                    options = (int)arg.Value.Value!;
                    break;
                case "MethodImpl":
                    methodImpl = (int)arg.Value.Value!;
                    break;
            }
        }

        return (options, methodImpl);
    }

    private static void GenerateAliasCode(
        SourceProductionContext context,
        AliasModel model)
    {
        var generator = new AliasCodeGenerator(model);
        var source = generator.Generate();

        var fileName = $"{model.TypeDisplayString.Replace(".", "_").Replace("<", "_").Replace(">", "_")}.g.cs";
        context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
    }
}
