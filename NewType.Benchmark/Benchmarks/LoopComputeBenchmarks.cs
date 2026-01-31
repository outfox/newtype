using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

/// <summary>
/// Tight loops over arrays — tests that JIT produces identical loop bodies,
/// bounds-check elimination, and register allocation for raw vs alias types.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
[ShortRunJob]
public class LoopComputeBenchmarks
{
    private const int N = 1024;

    private int[] _rawArr = null!;
    private EntityId[] _aliasArr = null!;
    private int[] _rawScoreArr = null!;
    private Score[] _scoreArr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawArr = new int[N];
        _aliasArr = new EntityId[N];
        _rawScoreArr = new int[N];
        _scoreArr = new Score[N];

        for (var i = 0; i < N; i++)
        {
            _rawArr[i] = i + 1;
            _aliasArr[i] = i + 1;
            _rawScoreArr[i] = i + 1;
            _scoreArr[i] = i + 1;
        }
    }

    // --- Sum ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sum")]
    public int Sum_Raw()
    {
        var sum = 0;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("Sum")]
    public EntityId Sum_Alias()
    {
        EntityId sum = 0;
        var arr = _aliasArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    // --- Sum (record struct) ---
    [Benchmark(Baseline = true), BenchmarkCategory("SumRecord")]
    public int SumRecord_Raw()
    {
        var sum = 0;
        var arr = _rawScoreArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("SumRecord")]
    public Score SumRecord_Alias()
    {
        Score sum = 0;
        var arr = _scoreArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];
        return sum;
    }

    // --- Min ---
    [Benchmark(Baseline = true), BenchmarkCategory("Min")]
    public int Min_Raw()
    {
        var arr = _rawArr;
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i] < min) min = arr[i];
        return min;
    }

    [Benchmark, BenchmarkCategory("Min")]
    public EntityId Min_Alias()
    {
        var arr = _aliasArr;
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i] < min) min = arr[i];
        return min;
    }

    // --- Multiply-accumulate: sum += arr[i] * i ---
    [Benchmark(Baseline = true), BenchmarkCategory("MulAcc")]
    public int MulAcc_Raw()
    {
        var sum = 0;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i] * i;
        return sum;
    }

    [Benchmark, BenchmarkCategory("MulAcc")]
    public EntityId MulAcc_Alias()
    {
        EntityId sum = 0;
        var arr = _aliasArr;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i] * i;
        return sum;
    }
}
