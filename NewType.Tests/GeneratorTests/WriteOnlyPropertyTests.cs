using Xunit;

namespace newtype.tests;

public class WriteOnlyPropertyTests
{
    [Fact]
    public void Write_Only_Static_Property_Is_Excluded()
    {
        const string source = """
            using newtype;

            public class Config
            {
                public static string Mode { set { } }
                public static string Name { get; set; }
            }

            [newtype<Config>]
            public readonly partial struct ConfigAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("ConfigAlias.g.cs"))
            .SourceText.ToString();

        // Name has a getter and should be forwarded
        Assert.Contains("Name", text);
        // Mode is write-only and should not be forwarded
        Assert.DoesNotContain("Mode", text);
    }

    [Fact]
    public void Write_Only_Instance_Property_Is_Excluded()
    {
        const string source = """
            using newtype;

            public class Sensor
            {
                public int Calibration { set { } }
                public int Reading { get; }
                public Sensor(int reading) => Reading = reading;
            }

            [newtype<Sensor>]
            public readonly partial struct SensorAlias;
            """;

        var result = GeneratorTestHelper.RunGenerator(source);
        var text = result.Results[0].GeneratedSources
            .Single(s => s.HintName.EndsWith("SensorAlias.g.cs"))
            .SourceText.ToString();

        // Reading has a getter and should be forwarded
        Assert.Contains("Reading", text);
        // Calibration is write-only and should not be forwarded
        Assert.DoesNotContain("Calibration", text);
    }
}
