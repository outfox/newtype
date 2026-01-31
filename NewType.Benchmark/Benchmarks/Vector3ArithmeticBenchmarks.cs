using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 2)]
[ShortRunJob]
public class Vector3ArithmeticBenchmarks
{
    private Vector3 _rawA, _rawB;
    private float _scalar;
    private Position _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _rawA = new Vector3(t, t + 1, t + 2);
        _rawB = new Vector3(t + 3, t + 4, t + 5);
        _scalar = t * 0.01f;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public Vector3 Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public Position Add_Alias() => _aliasA + _aliasB;

    // --- Subtraction ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sub")]
    public Vector3 Sub_Raw() => _rawA - _rawB;

    [Benchmark, BenchmarkCategory("Sub")]
    public Position Sub_Alias() => _aliasA - _aliasB;

    // --- Scalar Multiply ---
    [Benchmark(Baseline = true), BenchmarkCategory("ScalarMul")]
    public Vector3 ScalarMul_Raw() => _rawA * _scalar;

    [Benchmark, BenchmarkCategory("ScalarMul")]
    public Position ScalarMul_Alias() => _aliasA * _scalar;

    // --- Negation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neg")]
    public Vector3 Neg_Raw() => -_rawA;

    [Benchmark, BenchmarkCategory("Neg")]
    public Position Neg_Alias() => -_aliasA;

    // --- Equality ---
    [Benchmark(Baseline = true), BenchmarkCategory("Eq")]
    public bool Eq_Raw() => _rawA == _rawB;

    [Benchmark, BenchmarkCategory("Eq")]
    public bool Eq_Alias() => _aliasA == _aliasB;
}
