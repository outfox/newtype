using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class RecordStructBenchmarks
{
    private int _rawA, _rawB;
    private Score _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        _rawA = Environment.TickCount;
        _rawB = Environment.TickCount ^ 0x5DEECE6;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public int Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public Score Add_Alias() => _aliasA + _aliasB;

    // --- Multiplication ---
    [Benchmark(Baseline = true), BenchmarkCategory("Mul")]
    public int Mul_Raw() => _rawA * _rawB;

    [Benchmark, BenchmarkCategory("Mul")]
    public Score Mul_Alias() => _aliasA * _aliasB;

    // --- Equality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Eq")]
    public bool Eq_Raw() => _rawA == _rawB;

    [Benchmark, BenchmarkCategory("Eq")]
    public bool Eq_Alias() => _aliasA == _aliasB;

    // --- GetHashCode ---
    [Benchmark(Baseline = true), BenchmarkCategory("GetHashCode")]
    public int GetHashCode_Raw() => _rawA.GetHashCode();

    [Benchmark, BenchmarkCategory("GetHashCode")]
    public int GetHashCode_Alias() => _aliasA.GetHashCode();

    // --- CompareTo ---
    [Benchmark(Baseline = true), BenchmarkCategory("CompareTo")]
    public int CompareTo_Raw() => _rawA.CompareTo(_rawB);

    [Benchmark, BenchmarkCategory("CompareTo")]
    public int CompareTo_Alias() => _aliasA.CompareTo(_aliasB);
}
