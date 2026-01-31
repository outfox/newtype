using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PrimitiveComparisonBenchmarks
{
    private int _rawA, _rawB;
    private EntityId _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        _rawA = Environment.TickCount;
        _rawB = Environment.TickCount ^ 0x5DEECE6;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Less Than ---
    [Benchmark(Baseline = true), BenchmarkCategory("Lt")]
    public bool Lt_Raw() => _rawA < _rawB;

    [Benchmark, BenchmarkCategory("Lt")]
    public bool Lt_Alias() => _aliasA < _aliasB;

    // --- Greater Than ---
    [Benchmark(Baseline = true), BenchmarkCategory("Gt")]
    public bool Gt_Raw() => _rawA > _rawB;

    [Benchmark, BenchmarkCategory("Gt")]
    public bool Gt_Alias() => _aliasA > _aliasB;

    // --- CompareTo ---
    [Benchmark(Baseline = true), BenchmarkCategory("CompareTo")]
    public int CompareTo_Raw() => _rawA.CompareTo(_rawB);

    [Benchmark, BenchmarkCategory("CompareTo")]
    public int CompareTo_Alias() => _aliasA.CompareTo(_aliasB);
}
