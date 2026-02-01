using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

/// <summary>
/// Compares AggressiveInlining (EntityId) vs default MethodImpl (DefaultImplId) vs raw int
/// across loop-based workloads to measure whether the JIT inlines both equally.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
[ShortRunJob]
public class MethodImplBenchmarks
{
    private const int N = 1024;

    private int[] _rawArr = null!;
    private EntityId[] _inlinedArr = null!;
    private DefaultImplId[] _defaultArr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawArr = new int[N];
        _inlinedArr = new EntityId[N];
        _defaultArr = new DefaultImplId[N];

        for (var i = 0; i < N; i++)
        {
            _rawArr[i] = i + 1;
            _inlinedArr[i] = new EntityId(i + 1);
            _defaultArr[i] = new DefaultImplId(i + 1);
        }
    }

    // --- Sum (addition loop) ---
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
    public EntityId Sum_Inlined()
    {
        var sum = new EntityId(0);
        var arr = _inlinedArr;
        for (var i = 0; i < arr.Length; i++)
            sum = sum + arr[i];
        return sum;
    }

    [Benchmark, BenchmarkCategory("Sum")]
    public DefaultImplId Sum_Default()
    {
        var sum = new DefaultImplId(0);
        var arr = _defaultArr;
        for (var i = 0; i < arr.Length; i++)
            sum = sum + arr[i];
        return sum;
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
    public EntityId MulAcc_Inlined()
    {
        var sum = new EntityId(0);
        var arr = _inlinedArr;
        for (var i = 0; i < arr.Length; i++)
            sum = sum + arr[i] * new EntityId(i);
        return sum;
    }

    [Benchmark, BenchmarkCategory("MulAcc")]
    public DefaultImplId MulAcc_Default()
    {
        var sum = new DefaultImplId(0);
        var arr = _defaultArr;
        for (var i = 0; i < arr.Length; i++)
            sum = sum + arr[i] * new DefaultImplId(i);
        return sum;
    }

    // --- Min (comparison loop) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Min")]
    public int Min_Raw()
    {
        var arr = _rawArr;
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i] < min)
                min = arr[i];
        return min;
    }

    [Benchmark, BenchmarkCategory("Min")]
    public EntityId Min_Inlined()
    {
        var arr = _inlinedArr;
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i] < min)
                min = arr[i];
        return min;
    }

    [Benchmark, BenchmarkCategory("Min")]
    public DefaultImplId Min_Default()
    {
        var arr = _defaultArr;
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i] < min)
                min = arr[i];
        return min;
    }

    // --- XOR fold (bitwise loop) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Xor")]
    public int Xor_Raw()
    {
        var acc = 0;
        var arr = _rawArr;
        for (var i = 0; i < arr.Length; i++)
            acc ^= arr[i];
        return acc;
    }

    [Benchmark, BenchmarkCategory("Xor")]
    public EntityId Xor_Inlined()
    {
        var acc = new EntityId(0);
        var arr = _inlinedArr;
        for (var i = 0; i < arr.Length; i++)
            acc = acc ^ arr[i];
        return acc;
    }

    [Benchmark, BenchmarkCategory("Xor")]
    public DefaultImplId Xor_Default()
    {
        var acc = new DefaultImplId(0);
        var arr = _defaultArr;
        for (var i = 0; i < arr.Length; i++)
            acc = acc ^ arr[i];
        return acc;
    }
}
