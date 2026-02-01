using System.Globalization;
using Xunit;

namespace newtype.tests;

public class DefaultValueFormattingTests
{
    [Fact]
    public void Decimal_Default_Value_Uses_Invariant_Culture()
    {
        const string source = """
            using newtype;

            public class Currency
            {
                public decimal Amount { get; }
                public Currency(decimal amount = 1.5m) => Amount = amount;
            }

            [newtype<Currency>]
            public readonly partial struct Price;
            """;

        // Run under de-DE where the decimal separator is a comma
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var result = GeneratorTestHelper.RunGenerator(source);
            var text = result.Results[0].GeneratedSources
                .Single(s => s.HintName.EndsWith("Price.g.cs"))
                .SourceText.ToString();

            // Must use period even under de-DE culture
            Assert.Contains("1.5m", text);
            Assert.DoesNotContain("1,5m", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Float_Default_Value_Uses_Invariant_Culture()
    {
        const string source = """
            using newtype;

            public class Sensor
            {
                public float Reading { get; }
                public Sensor(float reading = 2.5f) => Reading = reading;
            }

            [newtype<Sensor>]
            public readonly partial struct SensorAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("SensorAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains("2.5f", text);
    }

    [Fact]
    public void Double_Default_Value_Uses_Invariant_Culture()
    {
        const string source = """
            using newtype;

            public class Measurement
            {
                public double Result { get; }
                public Measurement(double result = 3.14d) => Result = result;
            }

            [newtype<Measurement>]
            public readonly partial struct MeasurementAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("MeasurementAlias.g.cs"))
            .SourceText.ToString();

        Assert.Contains("3.14d", text);
    }
}
