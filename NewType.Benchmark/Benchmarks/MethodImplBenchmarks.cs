using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 2)]
[ShortRunJob]
public class MethodImplBenchmarks
{
    private int _rawA, _rawB;
    private EntityId _inlinedA, _inlinedB;
    private DefaultImplId _defaultA, _defaultB;

    [GlobalSetup]
    public void Setup()
    {
        _rawA = Environment.TickCount;
        _rawB = Environment.TickCount ^ 0x5DEECE6;
        _inlinedA = _rawA;
        _inlinedB = _rawB;
        _defaultA = new DefaultImplId(_rawA);
        _defaultB = new DefaultImplId(_rawB);
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public int Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public EntityId Add_Inlined() => _inlinedA + _inlinedB;

    [Benchmark, BenchmarkCategory("Add")]
    public DefaultImplId Add_Default() => _defaultA + _defaultB;

    // --- Subtraction ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sub")]
    public int Sub_Raw() => _rawA - _rawB;

    [Benchmark, BenchmarkCategory("Sub")]
    public EntityId Sub_Inlined() => _inlinedA - _inlinedB;

    [Benchmark, BenchmarkCategory("Sub")]
    public DefaultImplId Sub_Default() => _defaultA - _defaultB;

    // --- Comparison ---
    [Benchmark(Baseline = true), BenchmarkCategory("Cmp")]
    public bool Cmp_Raw() => _rawA < _rawB;

    [Benchmark, BenchmarkCategory("Cmp")]
    public bool Cmp_Inlined() => _inlinedA < _inlinedB;

    [Benchmark, BenchmarkCategory("Cmp")]
    public bool Cmp_Default() => _defaultA < _defaultB;

    // --- Construction + Conversion ---
    [Benchmark(Baseline = true), BenchmarkCategory("Ctor")]
    public int Ctor_Raw() => _rawA;

    [Benchmark, BenchmarkCategory("Ctor")]
    public int Ctor_Inlined() => new EntityId(_rawA).Value;

    [Benchmark, BenchmarkCategory("Ctor")]
    public int Ctor_Default() => new DefaultImplId(_rawA).Value;

    // --- Loop accumulation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Loop")]
    public int Loop_Raw()
    {
        int sum = 0;
        for (int i = 0; i < 1000; i++)
            sum += i;
        return sum;
    }

    [Benchmark, BenchmarkCategory("Loop")]
    public EntityId Loop_Inlined()
    {
        EntityId sum = new EntityId(0);
        for (int i = 0; i < 1000; i++)
            sum = sum + new EntityId(i);
        return sum;
    }

    [Benchmark, BenchmarkCategory("Loop")]
    public DefaultImplId Loop_Default()
    {
        DefaultImplId sum = new DefaultImplId(0);
        for (int i = 0; i < 1000; i++)
            sum = sum + new DefaultImplId(i);
        return sum;
    }
}
