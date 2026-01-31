using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace newtype.benchmark;

[MemoryDiagnoser(displayGenColumns: false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[ShortRunJob]
public class ConversionBenchmarks
{
    private int _intVal;
    private EntityId _entityId;
    private Vector3 _vec3Val;
    private Position _position;
    private Score _score;

    [GlobalSetup]
    public void Setup()
    {
        var t = Environment.TickCount;
        _intVal = t;
        _entityId = _intVal;
        _vec3Val = new Vector3(t, t + 1, t + 2);
        _position = _vec3Val;
        _score = t;
    }

    // --- Primitive: T -> Alias ---
    [Benchmark(Baseline = true), BenchmarkCategory("Primitive_ToAlias")]
    public int ToAlias_Primitive_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("Primitive_ToAlias")]
    public EntityId ToAlias_Primitive_Alias() => _intVal;

    // --- Primitive: Alias -> T ---
    [Benchmark(Baseline = true), BenchmarkCategory("Primitive_FromAlias")]
    public int FromAlias_Primitive_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("Primitive_FromAlias")]
    public int FromAlias_Primitive_Alias() => _entityId;

    // --- Vector3: T -> Alias ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vector3_ToAlias")]
    public Vector3 ToAlias_Vector3_Raw() => _vec3Val;

    [Benchmark, BenchmarkCategory("Vector3_ToAlias")]
    public Position ToAlias_Vector3_Alias() => _vec3Val;

    // --- Vector3: Alias -> T ---
    [Benchmark(Baseline = true), BenchmarkCategory("Vector3_FromAlias")]
    public Vector3 FromAlias_Vector3_Raw() => _vec3Val;

    [Benchmark, BenchmarkCategory("Vector3_FromAlias")]
    public Vector3 FromAlias_Vector3_Alias() => _position;

    // --- Record Struct: T -> Alias ---
    [Benchmark(Baseline = true), BenchmarkCategory("RecordStruct_ToAlias")]
    public int ToAlias_RecordStruct_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("RecordStruct_ToAlias")]
    public Score ToAlias_RecordStruct_Alias() => _intVal;

    // --- Record Struct: Alias -> T ---
    [Benchmark(Baseline = true), BenchmarkCategory("RecordStruct_FromAlias")]
    public int FromAlias_RecordStruct_Raw() => _intVal;

    [Benchmark, BenchmarkCategory("RecordStruct_FromAlias")]
    public int FromAlias_RecordStruct_Alias() => _score;
}
