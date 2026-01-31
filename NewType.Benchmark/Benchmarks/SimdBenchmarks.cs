using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

/// <summary>
/// Vector3 array processing — tests that JIT generates identical SIMD
/// instructions (vaddps, vmulps, etc.) for raw vs alias array loops.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
public class SimdBenchmarks
{
    private const int N = 1024;

    private Vector3[] _rawArr = null!;
    private Position[] _aliasArr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawArr = new Vector3[N];
        _aliasArr = new Position[N];
        for (var i = 0; i < N; i++)
        {
            var v = new Vector3(i, i + 0.5f, i + 1.0f);
            _rawArr[i] = v;
            _aliasArr[i] = v;
        }
    }

    // --- Sum all vectors ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sum")]
    public Vector3 Sum_Raw()
    {
        var sum = Vector3.Zero;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("Sum")]
    public Position Sum_Alias()
    {
        Position sum = Vector3.Zero;
        var arr = _aliasArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    // --- Scale + offset: arr[i] * s + offset ---
    [Benchmark(Baseline = true), BenchmarkCategory("ScaleOffset")]
    public Vector3 ScaleOffset_Raw()
    {
        var sum = Vector3.Zero;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i] * 0.5f + Vector3.One;
        return sum;
    }

    [Benchmark, BenchmarkCategory("ScaleOffset")]
    public Position ScaleOffset_Alias()
    {
        Position sum = Vector3.Zero;
        var arr = _aliasArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i] * 0.5f + (Position)Vector3.One;
        return sum;
    }

    // --- Pairwise distance accumulation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Distance")]
    public float Distance_Raw()
    {
        var sum = 0f;
        var arr = _rawArr;
        for (var i = 1; i < arr.Length; i++)
            sum += (arr[i] - arr[i - 1]).Length();
        return sum;
    }

    [Benchmark, BenchmarkCategory("Distance")]
    public float Distance_Alias()
    {
        var sum = 0f;
        var arr = _aliasArr;
        for (var i = 1; i < arr.Length; i++)
            sum += (arr[i] - arr[i - 1]).Length();
        return sum;
    }

    // --- Dot product accumulation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Dot")]
    public float Dot_Raw()
    {
        var sum = 0f;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            sum += Vector3.Dot(arr[i], arr[i]);
        return sum;
    }

    [Benchmark, BenchmarkCategory("Dot")]
    public float Dot_Alias()
    {
        var sum = 0f;
        var arr = _aliasArr;
        for (var i = 0; i < arr.Length; i++)
            sum += Vector3.Dot(arr[i], arr[i]);
        return sum;
    }
}
