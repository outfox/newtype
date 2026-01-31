using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using newtype.generator;
using Xunit;

namespace newtype.tests;

internal static class GeneratorTestHelper
{
    public static GeneratorDriverRunResult RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            "Tests",
            [syntaxTree],
            Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new AliasGenerator().AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator],
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview),
            driverOptions: new GeneratorDriverOptions(
                disabledOutputs: IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true));

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // No generator diagnostics
        Assert.Empty(diagnostics);

        // No errors in the output compilation
        var errors = outputCompilation.GetDiagnostics()
            .Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
            .ToArray();
        Assert.Empty(errors);

        return driver.GetRunResult();
    }

    public static void VerifyIncrementality(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            "Tests",
            [syntaxTree],
            Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new AliasGenerator().AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator],
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview),
            driverOptions: new GeneratorDriverOptions(
                disabledOutputs: IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true));

        // First run
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        // Second run with an unrelated change (add a dummy syntax tree)
        var dummyTree = CSharpSyntaxTree.ParseText("// dummy", new CSharpParseOptions(LanguageVersion.Preview));
        var modifiedCompilation = compilation.AddSyntaxTrees(dummyTree);

        driver = driver.RunGeneratorsAndUpdateCompilation(modifiedCompilation, out _, out _);

        var result = driver.GetRunResult();

        // All output steps should be Cached or Unchanged on the second run
        foreach (var generatorResult in result.Results)
        {
            foreach (var (_, steps) in generatorResult.TrackedOutputSteps)
            {
                foreach (var step in steps)
                {
                    foreach (var output in step.Outputs)
                    {
                        Assert.True(
                            output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
                            $"Expected Cached or Unchanged but got {output.Reason}");
                    }
                }
            }
        }
    }
}
