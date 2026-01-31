using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

/// <summary>
/// Generic methods with struct constraints — tests that JIT devirtualizes
/// IEquatable/IComparable calls identically for raw and alias types.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
[ShortRunJob]
public class GenericConstraintBenchmarks
{
    private const int N = 1024;

    private int[] _rawArr = null!;
    private EntityId[] _aliasArr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawArr = new int[N];
        _aliasArr = new EntityId[N];
        for (var i = 0; i < N; i++)
        {
            _rawArr[i] = i;
            _aliasArr[i] = i;
        }
    }

    // --- Generic linear search (IEquatable<T>) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Contains")]
    public bool Contains_Raw() => Contains(_rawArr, 512);

    [Benchmark, BenchmarkCategory("Contains")]
    public bool Contains_Alias() => Contains(_aliasArr, (EntityId) 512);

    // --- Generic min (IComparable<T>) ---
    [Benchmark(Baseline = true), BenchmarkCategory("GenericMin")]
    public int GenericMin_Raw() => FindMin(_rawArr);

    [Benchmark, BenchmarkCategory("GenericMin")]
    public EntityId GenericMin_Alias() => FindMin(_aliasArr);

    // --- Generic sort (IComparable<T>) ---
    private int[] _rawSortBuf = null!;
    private EntityId[] _aliasSortBuf = null!;

    [IterationSetup(Targets = [nameof(GenericSort_Raw), nameof(GenericSort_Alias)])]
    public void SortSetup()
    {
        _rawSortBuf = new int[N];
        _aliasSortBuf = new EntityId[N];
        // Reverse order for worst-case
        for (var i = 0; i < N; i++)
        {
            _rawSortBuf[i] = N - i;
            _aliasSortBuf[i] = N - i;
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("GenericSort")]
    public void GenericSort_Raw() => InsertionSort(_rawSortBuf);

    [Benchmark, BenchmarkCategory("GenericSort")]
    public void GenericSort_Alias() => InsertionSort(_aliasSortBuf);

    // --- Generic count matches (IEquatable<T>) ---
    [Benchmark(Baseline = true), BenchmarkCategory("CountEq")]
    public int CountEq_Raw() => CountEqual(_rawArr, 0);

    [Benchmark, BenchmarkCategory("CountEq")]
    public int CountEq_Alias() => CountEqual(_aliasArr, (EntityId) 0);

    // --- Helpers: generic with struct constraints for devirtualization ---

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Contains<T>(T[] arr, T value) where T : struct, IEquatable<T>
    {
        for (var i = 0; i < arr.Length; i++)
            if (arr[i].Equals(value))
                return true;
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T FindMin<T>(T[] arr) where T : struct, IComparable<T>
    {
        var min = arr[0];
        for (var i = 1; i < arr.Length; i++)
            if (arr[i].CompareTo(min) < 0)
                min = arr[i];
        return min;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InsertionSort<T>(T[] arr) where T : struct, IComparable<T>
    {
        for (var i = 1; i < arr.Length; i++)
        {
            var key = arr[i];
            var j = i - 1;
            while (j >= 0 && arr[j].CompareTo(key) > 0)
            {
                arr[j + 1] = arr[j];
                j--;
            }

            arr[j + 1] = key;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CountEqual<T>(T[] arr, T value) where T : struct, IEquatable<T>
    {
        var count = 0;
        for (var i = 0; i < arr.Length; i++)
            if (arr[i].Equals(value))
                count++;
        return count;
    }
}