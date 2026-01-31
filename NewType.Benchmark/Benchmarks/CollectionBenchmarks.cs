using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[ShortRunJob]
public class CollectionBenchmarks
{
    private const int N = 1000;

    private Dictionary<int, int> _rawDict = null!;
    private Dictionary<EntityId, int> _aliasDict = null!;
    private int _rawKey;
    private EntityId _aliasKey;

    private int[] _rawArray = null!;
    private EntityId[] _aliasArray = null!;
    private int[] _rawSortBuffer = null!;
    private EntityId[] _aliasSortBuffer = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(Environment.TickCount);
        _rawDict = new Dictionary<int, int>(N);
        _aliasDict = new Dictionary<EntityId, int>(N);

        for (var i = 0; i < N; i++)
        {
            _rawDict[i] = i;
            _aliasDict[(EntityId) i] = i;
        }

        _rawKey = N / 2;
        _aliasKey = _rawKey;

        _rawArray = new int[N];
        _aliasArray = new EntityId[N];
        for (var i = 0; i < N; i++)
        {
            var v = rng.Next();
            _rawArray[i] = v;
            _aliasArray[i] = v;
        }

        _rawSortBuffer = new int[N];
        _aliasSortBuffer = new EntityId[N];
    }

    // --- Dictionary Lookup ---
    [Benchmark(Baseline = true), BenchmarkCategory("DictLookup")]
    public int DictLookup_Raw() => _rawDict[_rawKey];

    [Benchmark, BenchmarkCategory("DictLookup")]
    public int DictLookup_Alias() => _aliasDict[_aliasKey];

    // --- Dictionary TryGetValue ---
    [Benchmark(Baseline = true), BenchmarkCategory("DictTryGet")]
    public bool DictTryGet_Raw() => _rawDict.TryGetValue(_rawKey, out _);

    [Benchmark, BenchmarkCategory("DictTryGet")]
    public bool DictTryGet_Alias() => _aliasDict.TryGetValue(_aliasKey, out _);

    // --- Array.Sort ---
    [IterationSetup(Targets = [nameof(Sort_Raw), nameof(Sort_Alias)])]
    public void SortSetup()
    {
        Array.Copy(_rawArray, _rawSortBuffer, N);
        Array.Copy(_aliasArray, _aliasSortBuffer, N);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Sort")]
    public void Sort_Raw() => Array.Sort(_rawSortBuffer);

    [Benchmark, BenchmarkCategory("Sort")]
    public void Sort_Alias() => Array.Sort(_aliasSortBuffer);
}