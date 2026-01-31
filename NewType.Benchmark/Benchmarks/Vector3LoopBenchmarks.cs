using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
[ShortRunJob]
public class Vector3LoopBenchmarks
{
    private const int N = 1024;
    private Vector3[] _rawArray = null!;
    private Position[] _aliasArray = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawArray = new Vector3[N];
        _aliasArray = new Position[N];
        for (int i = 0; i < N; i++)
        {
            _rawArray[i] = new Vector3(i, i + 1, i + 2);
            _aliasArray[i] = _rawArray[i];
        }
    }

    // --- Sum reduction (Add in a tight loop) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public Vector3 Add_Raw()
    {
        var sum = Vector3.Zero;
        var arr = _rawArray;
        for (int i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("Add")]
    public Position Add_Alias()
    {
        var sum = Position.Zero;
        var arr = _aliasArray;
        for (int i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    // --- Negate all elements and sum ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neg")]
    public Vector3 Neg_Raw()
    {
        var sum = Vector3.Zero;
        var arr = _rawArray;
        for (int i = 0; i < arr.Length; i++)
            sum += -arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("Neg")]
    public Position Neg_Alias()
    {
        var sum = Position.Zero;
        var arr = _aliasArray;
        for (int i = 0; i < arr.Length; i++)
            sum += -arr[i];
        return sum;
    }

    // --- Scale + offset (chained ops) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Chain")]
    public Vector3 Chain_Raw()
    {
        var acc = Vector3.Zero;
        var offset = new Vector3(1, 2, 3);
        var arr = _rawArray;
        for (int i = 0; i < arr.Length; i++)
            acc += arr[i] * 2.0f + offset;
        return acc;
    }

    [Benchmark, BenchmarkCategory("Chain")]
    public Position Chain_Alias()
    {
        var acc = Position.Zero;
        Position offset = new Vector3(1, 2, 3);
        var arr = _aliasArray;
        for (int i = 0; i < arr.Length; i++)
            acc += arr[i] * 2.0f + offset;
        return acc;
    }
}
