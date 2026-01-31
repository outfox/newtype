using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace newtype.benchmark;

/// <summary>
/// Chained arithmetic expressions that let the JIT inline aggressively.
/// Identical disassembly between raw and alias proves zero-cost abstraction.
/// </summary>
[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[DisassemblyDiagnoser(maxDepth: 3)]
[ShortRunJob]
public class InliningChainBenchmarks
{
    private int _a, _b, _c, _d, _e;
    private EntityId _aA, _bA, _cA, _dA, _eA;
    private Vector3 _v1, _v2, _v3;
    private Position _p1, _p2, _p3;
    private float _s;

    [GlobalSetup]
    public void Setup()
    {
        _a = 17; _b = 42; _c = 9; _d = 3; _e = 100;
        _aA = _a; _bA = _b; _cA = _c; _dA = _d; _eA = _e;
        _v1 = new Vector3(1, 2, 3);
        _v2 = new Vector3(4, 5, 6);
        _v3 = new Vector3(7, 8, 9);
        _p1 = _v1; _p2 = _v2; _p3 = _v3;
        _s = 0.5f;
    }

    // --- (a + b) * c - d ---
    [Benchmark(Baseline = true), BenchmarkCategory("Chain3")]
    public int Chain3_Raw() => (_a + _b) * _c - _d;

    [Benchmark, BenchmarkCategory("Chain3")]
    public EntityId Chain3_Alias() => (_aA + _bA) * _cA - _dA;

    // --- ((a + b) * (c - d)) ^ e ---
    [Benchmark(Baseline = true), BenchmarkCategory("Chain5")]
    public int Chain5_Raw() => ((_a + _b) * (_c - _d)) ^ _e;

    [Benchmark, BenchmarkCategory("Chain5")]
    public EntityId Chain5_Alias() => ((_aA + _bA) * (_cA - _dA)) ^ _eA;

    // --- (a * b + c) * (d - e) + (a ^ c) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Chain6")]
    public int Chain6_Raw() => (_a * _b + _c) * (_d - _e) + (_a ^ _c);

    [Benchmark, BenchmarkCategory("Chain6")]
    public EntityId Chain6_Alias() => (_aA * _bA + _cA) * (_dA - _eA) + (_aA ^ _cA);

    // --- Vector3: (v1 + v2) * s - v3 ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vec3Chain")]
    public Vector3 Vec3Chain_Raw() => (_v1 + _v2) * _s - _v3;

    [Benchmark, BenchmarkCategory("Vec3Chain")]
    public Position Vec3Chain_Alias() => (_p1 + _p2) * _s - _p3;

    // --- Vector3: v1 + v2 + v3 (associative chain) ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vec3Sum")]
    public Vector3 Vec3Sum_Raw() => _v1 + _v2 + _v3;

    [Benchmark, BenchmarkCategory("Vec3Sum")]
    public Position Vec3Sum_Alias() => _p1 + _p2 + _p3;
}
