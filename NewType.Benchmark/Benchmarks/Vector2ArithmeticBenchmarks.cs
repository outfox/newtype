using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 2)]
[ShortRunJob]
public class Vector2ArithmeticBenchmarks
{
    private Vector2 _rawA, _rawB;
    private float _scalar;
    private Position2 _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _rawA = new Vector2(t, t + 1);
        _rawB = new Vector2(t + 3, t + 4);
        _scalar = t * 0.01f;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public Vector2 Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public Position2 Add_Alias() => _aliasA + _aliasB;

    // --- Subtraction ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sub")]
    public Vector2 Sub_Raw() => _rawA - _rawB;

    [Benchmark, BenchmarkCategory("Sub")]
    public Position2 Sub_Alias() => _aliasA - _aliasB;

    // --- Scalar Multiply ---
    [Benchmark(Baseline = true), BenchmarkCategory("ScalarMul")]
    public Vector2 ScalarMul_Raw() => _rawA * _scalar;

    [Benchmark, BenchmarkCategory("ScalarMul")]
    public Position2 ScalarMul_Alias() => _aliasA * _scalar;

    // --- Negation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neg")]
    public Vector2 Neg_Raw() => -_rawA;

    [Benchmark, BenchmarkCategory("Neg")]
    public Position2 Neg_Alias() => -_aliasA;

    // --- Equality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Eq")]
    public bool Eq_Raw() => _rawA == _rawB;

    [Benchmark, BenchmarkCategory("Eq")]
    public bool Eq_Alias() => _aliasA == _aliasB;
}