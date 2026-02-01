using Xunit;

namespace newtype.tests;

public class ConstructorSignatureTests
{
    [Fact]
    public void Ref_And_Value_Constructors_Are_Not_Treated_As_Duplicates()
    {
        const string source = """
            using newtype;

            public class Parser
            {
                public string Result { get; }
                public Parser(string input) => Result = input;
                public Parser(ref string input) { Result = input; input = ""; }
            }

            [newtype<Parser>]
            public readonly partial struct ParserAlias;
            """;

        // Should compile without errors — both constructors should be forwarded
        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("ParserAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains("public ParserAlias(string input)", text);
    }
}
