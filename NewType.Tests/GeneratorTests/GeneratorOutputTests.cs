using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using newtype.generator;
using Xunit;

namespace newtype.tests;

public class GeneratorOutputTests
{
    [Fact]
    public void Generates_Output_For_Int_Alias()
    {
        const string source = """
            using newtype;

            [newtype<int>]
            public readonly partial struct TestId;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);

        // Attribute source + alias source
        Assert.Equal(2, result.GeneratedTrees.Length);

        var generatedSources = result.Results[0].GeneratedSources;

        var aliasSource = generatedSources.Single(s => s.HintName.EndsWith("TestId.g.cs"));
        var text = aliasSource.SourceText.ToString();

        Assert.Contains("private readonly int _value;", text);
        Assert.Contains("public TestId(int value)", text);
        Assert.Contains("operator +", text);
        Assert.Contains("operator ==", text);
        Assert.Contains("IComparable<TestId>", text);
        Assert.Contains("IEquatable<TestId>", text);
    }

    [Fact]
    public void Generates_Output_For_String_Alias()
    {
        const string source = """
            using newtype;

            [newtype<string>]
            public readonly partial struct Label;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);

        Assert.Equal(2, result.GeneratedTrees.Length);

        var generatedSources = result.Results[0].GeneratedSources;

        var aliasSource = generatedSources.Single(s => s.HintName.EndsWith("Label.g.cs"));
        var text = aliasSource.SourceText.ToString();

        Assert.Contains("private readonly string _value;", text);
        Assert.Contains(@"_value?.ToString() ?? """"", text);
        Assert.Contains("IComparable<Label>", text);
    }

    [Fact]
    public void Generates_Output_For_Custom_Class_Alias()
    {
        const string source = """
            using newtype;
            using System;

            public class Widget
            {
                public string Name { get; }

                public Widget(string name) => Name = name;

                public static Widget operator +(Widget a, Widget b) => new(a.Name + b.Name);
            }

            [newtype<Widget>]
            public readonly partial struct MyWidget;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);

        Assert.Equal(2, result.GeneratedTrees.Length);

        var generatedSources = result.Results[0].GeneratedSources;

        var aliasSource = generatedSources.Single(s => s.HintName.EndsWith("MyWidget.g.cs"));
        var text = aliasSource.SourceText.ToString();

        // Operator forwarding
        Assert.Contains("operator +", text);
        // Property forwarding
        Assert.Contains("Name", text);
        // Constructor forwarding
        Assert.Contains("public MyWidget(string name)", text);
    }

    [Fact]
    public void Non_Partial_Struct_Produces_Compilation_Error()
    {
        const string source = """
            using newtype;

            [newtype<int>]
            public struct Bad;
            """;

        // Run the generator directly — we expect a compilation error (CS0260),
        // so we can't use RunGenerator which asserts no errors.
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            "Tests",
            [syntaxTree],
            GeneratorTestHelper.TargetReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new AliasGenerator().AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator],
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview),
            driverOptions: new GeneratorDriverOptions(
                disabledOutputs: IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true));

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        // The generated partial declaration conflicts with the non-partial user declaration
        var errors = outputCompilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToArray();

        Assert.Contains(errors, e => e.Id == "CS0260");
    }

    [Fact]
    public void Generates_Separate_Outputs_Per_Type()
    {
        const string source = """
            using newtype;

            [newtype<int>]
            public readonly partial struct IdA;

            [newtype<float>]
            public readonly partial struct IdB;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);

        // Attribute + 2 aliases
        Assert.Equal(3, result.GeneratedTrees.Length);

        var generatedSources = result.Results[0].GeneratedSources;

        Assert.Single(generatedSources.Where(s => s.HintName.EndsWith("IdA.g.cs")));
        Assert.Single(generatedSources.Where(s => s.HintName.EndsWith("IdB.g.cs")));
    }

    [Fact]
    public void NonGeneric_Attribute_Works()
    {
        const string source = """
            using newtype;

            [newtype(typeof(int))]
            public readonly partial struct TestId;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);

        Assert.Equal(2, result.GeneratedTrees.Length);

        var generatedSources = result.Results[0].GeneratedSources;

        var aliasSource = generatedSources.Single(s => s.HintName.EndsWith("TestId.g.cs"));
        var text = aliasSource.SourceText.ToString();

        Assert.Contains("private readonly int _value;", text);
        Assert.Contains("public TestId(int value)", text);
    }
}
