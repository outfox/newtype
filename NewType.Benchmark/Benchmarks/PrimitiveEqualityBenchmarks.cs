using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PrimitiveEqualityBenchmarks
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

    // --- Equality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Eq")]
    public bool Eq_Raw() => _rawA == _rawB;

    [Benchmark, BenchmarkCategory("Eq")]
    public bool Eq_Alias() => _aliasA == _aliasB;

    // --- Inequality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neq")]
    public bool Neq_Raw() => _rawA != _rawB;

    [Benchmark, BenchmarkCategory("Neq")]
    public bool Neq_Alias() => _aliasA != _aliasB;

    // --- IEquatable<T>.Equals ---
    [Benchmark(Baseline = true), BenchmarkCategory("Equals")]
    public bool Equals_Raw() => _rawA.Equals(_rawB);

    [Benchmark, BenchmarkCategory("Equals")]
    public bool Equals_Alias() => _aliasA.Equals(_aliasB);
}
