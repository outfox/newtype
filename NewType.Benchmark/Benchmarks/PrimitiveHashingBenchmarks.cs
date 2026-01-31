using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[ShortRunJob]
public class PrimitiveHashingBenchmarks
{
    private int _rawA;
    private EntityId _aliasA;

    [GlobalSetup]
    public void Setup()
    {
        _rawA = Environment.TickCount;
        _aliasA = _rawA;
    }

    [Benchmark(Baseline = true), BenchmarkCategory("GetHashCode")]
    public int GetHashCode_Raw() => _rawA.GetHashCode();

    [Benchmark, BenchmarkCategory("GetHashCode")]
    public int GetHashCode_Alias() => _aliasA.GetHashCode();
}
