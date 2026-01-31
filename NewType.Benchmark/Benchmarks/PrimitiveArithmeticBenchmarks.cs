using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 2)]
public class PrimitiveArithmeticBenchmarks
{
    private int _rawA, _rawB;
    private EntityId _aliasA, _aliasB;

    [GlobalSetup]
    public void Setup()
    {
        _rawA = Environment.TickCount;
        _rawB = Environment.TickCount ^ 0x5DEECE6;
        _aliasA = _rawA;
        _aliasB = _rawB;
    }

    // --- Addition ---
    [Benchmark(Baseline = true), BenchmarkCategory("Add")]
    public int Add_Raw() => _rawA + _rawB;

    [Benchmark, BenchmarkCategory("Add")]
    public EntityId Add_Alias() => _aliasA + _aliasB;

    // --- Subtraction ---
    [Benchmark(Baseline = true), BenchmarkCategory("Sub")]
    public int Sub_Raw() => _rawA - _rawB;

    [Benchmark, BenchmarkCategory("Sub")]
    public EntityId Sub_Alias() => _aliasA - _aliasB;

    // --- Multiplication ---
    [Benchmark(Baseline = true), BenchmarkCategory("Mul")]
    public int Mul_Raw() => _rawA * _rawB;

    [Benchmark, BenchmarkCategory("Mul")]
    public EntityId Mul_Alias() => _aliasA * _aliasB;

    // --- Division ---
    [Benchmark(Baseline = true), BenchmarkCategory("Div")]
    public int Div_Raw() => _rawA / _rawB;

    [Benchmark, BenchmarkCategory("Div")]
    public EntityId Div_Alias() => _aliasA / _aliasB;

    // --- Modulo ---
    [Benchmark(Baseline = true), BenchmarkCategory("Mod")]
    public int Mod_Raw() => _rawA % _rawB;

    [Benchmark, BenchmarkCategory("Mod")]
    public EntityId Mod_Alias() => _aliasA % _aliasB;

    // --- Bitwise AND ---
    [Benchmark(Baseline = true), BenchmarkCategory("And")]
    public int And_Raw() => _rawA & _rawB;

    [Benchmark, BenchmarkCategory("And")]
    public EntityId And_Alias() => _aliasA & _aliasB;

    // --- Bitwise OR ---
    [Benchmark(Baseline = true), BenchmarkCategory("Or")]
    public int Or_Raw() => _rawA | _rawB;

    [Benchmark, BenchmarkCategory("Or")]
    public EntityId Or_Alias() => _aliasA | _aliasB;

    // --- Bitwise XOR ---
    [Benchmark(Baseline = true), BenchmarkCategory("Xor")]
    public int Xor_Raw() => _rawA ^ _rawB;

    [Benchmark, BenchmarkCategory("Xor")]
    public EntityId Xor_Alias() => _aliasA ^ _aliasB;

    // --- Left Shift ---
    [Benchmark(Baseline = true), BenchmarkCategory("Shl")]
    public int Shl_Raw() => _rawA << 3;

    [Benchmark, BenchmarkCategory("Shl")]
    public EntityId Shl_Alias() => _aliasA << 3;

    // --- Unary Negation ---
    [Benchmark(Baseline = true), BenchmarkCategory("Neg")]
    public int Neg_Raw() => -_rawA;

    [Benchmark, BenchmarkCategory("Neg")]
    public EntityId Neg_Alias() => -_aliasA;
}
