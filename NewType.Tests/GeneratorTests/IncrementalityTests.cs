using Xunit;

namespace newtype.tests;

public class IncrementalityTests
{
    [Fact]
    public void Generator_Is_Incremental_For_Unrelated_Change()
    {
        const string source = """
            using newtype;

            [newtype<int>]
            public readonly partial struct TestId;
            """;

        GeneratorTestHelper.VerifyIncrementality(source);
    }

    [Fact]
    public void Generator_Is_Incremental_For_Multiple_Types()
    {
        const string source = """
            using newtype;

            [newtype<int>]
            public readonly partial struct TestId;

            [newtype<string>]
            public readonly partial struct TestName;
            """;

        GeneratorTestHelper.VerifyIncrementality(source);
    }
}
