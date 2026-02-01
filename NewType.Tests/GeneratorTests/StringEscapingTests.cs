using Xunit;

namespace newtype.tests;

public class StringEscapingTests
{
    [Fact]
    public void String_Default_With_Newline_Is_Escaped()
    {
        const string source = """
            using newtype;

            public class Message
            {
                public string Text { get; }
                public Message(string text = "line1\nline2") => Text = text;
            }

            [newtype<Message>]
            public readonly partial struct Msg;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("Msg.g.cs"))
            .SourceText.ToString();

        Assert.Contains(@"""line1\nline2""", text);
        Assert.DoesNotContain("line1\nline2", text);
    }

    [Fact]
    public void String_Default_With_Tab_Is_Escaped()
    {
        const string source = """
            using newtype;

            public class Row
            {
                public string Data { get; }
                public Row(string data = "col1\tcol2") => Data = data;
            }

            [newtype<Row>]
            public readonly partial struct RowAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("RowAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains(@"""col1\tcol2""", text);
    }

    [Fact]
    public void String_Default_With_Quote_Is_Escaped()
    {
        const string source = """
            using newtype;

            public class Label
            {
                public string Text { get; }
                public Label(string text = "say \"hi\"") => Text = text;
            }

            [newtype<Label>]
            public readonly partial struct LabelAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("LabelAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains(@"say \""hi\""", text);
    }

    [Fact]
    public void Char_Default_With_Single_Quote_Is_Escaped()
    {
        const string source = """
            using newtype;

            public class Token
            {
                public char Delimiter { get; }
                public Token(char delimiter = '\'') => Delimiter = delimiter;
            }

            [newtype<Token>]
            public readonly partial struct TokenAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("TokenAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains(@"'\''", text);
    }
}
