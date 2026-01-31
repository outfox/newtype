using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[ShortRunJob]
public class Vector3PropertyAccessBenchmarks
{
    private Vector3 _raw;
    private Position _alias;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _raw = new Vector3(t, t + 1, t + 2);
        _alias = _raw;
    }

    // --- X field access ---
    [Benchmark(Baseline = true), BenchmarkCategory("FieldX")]
    public float FieldX_Raw() => _raw.X;

    [Benchmark, BenchmarkCategory("FieldX")]
    public float FieldX_Alias() => _alias.X;

    // --- Value property (identity conversion) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Value")]
    public Vector3 Value_Raw() => _raw;

    [Benchmark, BenchmarkCategory("Value")]
    public Vector3 Value_Alias() => _alias.Value;

    // --- Length() method ---
    [Benchmark(Baseline = true), BenchmarkCategory("Length")]
    public float Length_Raw() => _raw.Length();

    [Benchmark, BenchmarkCategory("Length")]
    public float Length_Alias() => _alias.Length();

    // --- LengthSquared() method ---
    [Benchmark(Baseline = true), BenchmarkCategory("LengthSquared")]
    public float LengthSquared_Raw() => _raw.LengthSquared();

    [Benchmark, BenchmarkCategory("LengthSquared")]
    public float LengthSquared_Alias() => _alias.LengthSquared();
}
