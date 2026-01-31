using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 2)]
[ShortRunJob]
public class Vector4ArithmeticBenchmarks
{
    private Vector4 _rawA, _rawB;
    private float _scalar;
    private Position4 _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _rawA = new Vector4(t, t + 1, t + 2, t + 3);
        _rawB = new Vector4(t + 4, t + 5, t + 6, t + 7);
        _scalar = t * 0.01f;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public Vector4 Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public Position4 Add_Alias() => _aliasA + _aliasB;

    // --- Subtraction ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sub")]
    public Vector4 Sub_Raw() => _rawA - _rawB;

    [Benchmark, BenchmarkCategory("Sub")]
    public Position4 Sub_Alias() => _aliasA - _aliasB;

    // --- Scalar Multiply ---
    [Benchmark(Baseline = true), BenchmarkCategory("ScalarMul")]
    public Vector4 ScalarMul_Raw() => _rawA * _scalar;

    [Benchmark, BenchmarkCategory("ScalarMul")]
    public Position4 ScalarMul_Alias() => _aliasA * _scalar;

    // --- Negation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neg")]
    public Vector4 Neg_Raw() => -_rawA;

    [Benchmark, BenchmarkCategory("Neg")]
    public Position4 Neg_Alias() => -_aliasA;

    // --- Equality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Eq")]
    public bool Eq_Raw() => _rawA == _rawB;

    [Benchmark, BenchmarkCategory("Eq")]
    public bool Eq_Alias() => _aliasA == _aliasB;
}