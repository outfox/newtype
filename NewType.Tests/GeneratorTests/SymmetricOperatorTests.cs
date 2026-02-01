using Xunit;

namespace newtype.tests;

public class SymmetricOperatorTests
{
    [Fact]
    public void Both_Sides_Aliased_Emits_Alias_Op_T_But_Not_T_Op_Alias()
    {
        // When both operator params are the aliased type (e.g. Money + Money),
        // the generator emits Alias op Alias and Alias op T but NOT T op Alias
        // to avoid ambiguity when multiple aliases share the same underlying type.
        const string source = """
            using newtype;

            public class Pair
            {
                public int X { get; }
                public Pair(int x) => X = x;
                public static Pair operator +(Pair a, Pair b) => new(a.X + b.X);
            }

            [newtype<Pair>]
            public readonly partial struct PairA;

            [newtype<Pair>]
            public readonly partial struct PairB;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var textA = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("PairA.g.cs"))
            .SourceText.ToString();

        // Alias op Alias
        Assert.Contains("operator +(PairA left, PairA right)", textA);
        // Alias op T
        Assert.Contains("operator +(PairA left, global::Pair right)", textA);
        // T op Alias must NOT be emitted (would cause ambiguity with PairB)
        Assert.DoesNotContain("operator +(global::Pair left, PairA right)", textA);
    }

    [Fact]
    public void Asymmetric_Left_Only_Emits_Alias_Op_Foreign()
    {
        // operator *(Money, decimal) — left is aliased type, right is not
        const string source = """
            using newtype;

            public class Money
            {
                public decimal Amount { get; }
                public Money(decimal amount) => Amount = amount;
                public static Money operator *(Money a, decimal factor) => new(a.Amount * factor);
            }

            [newtype<Money>]
            public readonly partial struct Price;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("Price.g.cs"))
            .SourceText.ToString();

        Assert.Contains("operator *(Price left, decimal right)", text);
    }

    [Fact]
    public void Asymmetric_Right_Only_Emits_Foreign_Op_Alias()
    {
        // operator *(decimal, Money) — left is not aliased type, right is
        const string source = """
            using newtype;

            public class Money
            {
                public decimal Amount { get; }
                public Money(decimal amount) => Amount = amount;
                public static Money operator *(decimal factor, Money a) => new(a.Amount * factor);
            }

            [newtype<Money>]
            public readonly partial struct Price;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("Price.g.cs"))
            .SourceText.ToString();

        Assert.Contains("operator *(decimal left, Price right)", text);
    }
}
