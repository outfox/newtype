using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[ShortRunJob]
public class ConstructionBenchmarks
{
    private int _intVal;
    private Vector3 _vec3Val;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _intVal = t;
        _vec3Val = new Vector3(t, t + 1, t + 2);
    }

    // --- Primitive constructor ---
    [Benchmark(Baseline = true), BenchmarkCategory("Primitive_Ctor")]
    public int Ctor_Primitive_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("Primitive_Ctor")]
    public EntityId Ctor_Primitive_Alias() => new EntityId(_intVal);

    // --- Primitive implicit conversion ---
    [Benchmark(Baseline = true), BenchmarkCategory("Primitive_Implicit")]
    public int Implicit_Primitive_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("Primitive_Implicit")]
    public EntityId Implicit_Primitive_Alias() => _intVal;

    // --- Vector3 constructor ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vector3_Ctor")]
    public Vector3 Ctor_Vector3_Raw() => _vec3Val;

    [Benchmark, BenchmarkCategory("Vector3_Ctor")]
    public Position Ctor_Vector3_Alias() => new Position(_vec3Val);

    // --- Vector3 implicit conversion ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vector3_Implicit")]
    public Vector3 Implicit_Vector3_Raw() => _vec3Val;

    [Benchmark, BenchmarkCategory("Vector3_Implicit")]
    public Position Implicit_Vector3_Alias() => _vec3Val;

    // --- Record struct constructor ---
    [Benchmark(Baseline = true), BenchmarkCategory("RecordStruct_Ctor")]
    public int Ctor_RecordStruct_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("RecordStruct_Ctor")]
    public Score Ctor_RecordStruct_Alias() => new Score(_intVal);

    // --- Record struct implicit conversion ---
    [Benchmark(Baseline = true), BenchmarkCategory("RecordStruct_Implicit")]
    public int Implicit_RecordStruct_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("RecordStruct_Implicit")]
    public Score Implicit_RecordStruct_Alias() => _intVal;
}
