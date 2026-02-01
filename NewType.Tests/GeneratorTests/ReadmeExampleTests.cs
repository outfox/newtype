using Xunit;

namespace newtype.tests;

public class ReadmeExampleTests
{
    private const string ReadmeSource = """
        using newtype;

        [newtype<string>]
        public readonly partial struct TableId;

        [newtype<int>]
        public readonly partial struct PizzasEaten;

        [newtype<double>]
        public readonly partial struct Fullness;

        class Guest
        {
            TableId table = "Table 1";
            PizzasEaten pizzasEaten;
            Fullness fullness;

            public void fillEmUp(Fullness threshold)
            {
                while (fullness < threshold)
                {
                    pizzasEaten++;
                    fullness += 0.1;
                }
            }
        }
        """;

    [Fact]
    public void Readme_Example_Compiles_Without_Errors()
    {
        var result = GeneratorTestHelper.RunGenerator(ReadmeSource);

        // Attribute + 3 aliases
        Assert.Equal(4, result.GeneratedTrees.Length);

        var sources = result.Results[0].GeneratedSources;
        Assert.Single(sources.Where(s => s.HintName.EndsWith("TableId.g.cs")));
        Assert.Single(sources.Where(s => s.HintName.EndsWith("PizzasEaten.g.cs")));
        Assert.Single(sources.Where(s => s.HintName.EndsWith("Fullness.g.cs")));
    }

    [Fact]
    public void TableId_Has_String_Backing_And_Implicit_Conversion()
    {
        var result = GeneratorTestHelper.RunGenerator(ReadmeSource);
        var text = GetGeneratedText(result, "TableId.g.cs");

        Assert.Contains("private readonly string _value;", text);
        Assert.Contains("public static implicit operator TableId(string value)", text);
        Assert.Contains("public static implicit operator string(TableId value)", text);
    }

    [Fact]
    public void PizzasEaten_Has_Int_Backing_And_Arithmetic()
    {
        var result = GeneratorTestHelper.RunGenerator(ReadmeSource);
        var text = GetGeneratedText(result, "PizzasEaten.g.cs");

        Assert.Contains("private readonly int _value;", text);
        // ++ works via implicit conversion to int and back
        Assert.Contains("public static implicit operator PizzasEaten(int value)", text);
        Assert.Contains("public static implicit operator int(PizzasEaten value)", text);
        Assert.Contains("operator +", text);
    }

    [Fact]
    public void Fullness_Has_Comparison_And_Addition()
    {
        var result = GeneratorTestHelper.RunGenerator(ReadmeSource);
        var text = GetGeneratedText(result, "Fullness.g.cs");

        Assert.Contains("private readonly double _value;", text);
        Assert.Contains("operator <", text);
        Assert.Contains("operator +", text);
    }

    private static string GetGeneratedText(
        Microsoft.CodeAnalysis.GeneratorDriverRunResult result,
        string hintNameSuffix)
    {
        return result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith(hintNameSuffix))
            .SourceText.ToString();
    }
}
