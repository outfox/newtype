```

BenchmarkDotNet v0.15.8, Linux CachyOS
AMD Ryzen 9 5900X 1.73GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method          | Categories | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|---------------- |----------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|----------:|------------:|
| Add_Raw         | Add        | 0.0024 ns | 0.0762 ns | 0.0042 ns | 0.0000 ns |     ? |       ? |      15 B |         - |           ? |
| Add_Alias       | Add        | 0.2985 ns | 0.4726 ns | 0.0259 ns | 0.3083 ns |     ? |       ? |      41 B |         - |           ? |
|                 |            |           |           |           |           |       |         |           |           |             |
| Eq_Raw          | Eq         | 0.2043 ns | 0.2601 ns | 0.0143 ns | 0.2069 ns |  1.00 |    0.09 |      25 B |         - |          NA |
| Eq_Alias        | Eq         | 0.3320 ns | 0.2861 ns | 0.0157 ns | 0.3269 ns |  1.63 |    0.12 |      29 B |         - |          NA |
|                 |            |           |           |           |           |       |         |           |           |             |
| Neg_Raw         | Neg        | 0.0525 ns | 0.5364 ns | 0.0294 ns | 0.0632 ns |  1.42 |    1.28 |      18 B |         - |          NA |
| Neg_Alias       | Neg        | 0.2686 ns | 0.2369 ns | 0.0130 ns | 0.2708 ns |  7.26 |    5.04 |      40 B |         - |          NA |
|                 |            |           |           |           |           |       |         |           |           |             |
| ScalarMul_Raw   | ScalarMul  | 0.0591 ns | 0.3080 ns | 0.0169 ns | 0.0496 ns |  1.05 |    0.35 |      20 B |         - |          NA |
| ScalarMul_Alias | ScalarMul  | 0.2307 ns | 0.2935 ns | 0.0161 ns | 0.2347 ns |  4.09 |    0.91 |      42 B |         - |          NA |
|                 |            |           |           |           |           |       |         |           |           |             |
| Sub_Raw         | Sub        | 0.0106 ns | 0.3096 ns | 0.0170 ns | 0.0016 ns |     ? |       ? |      15 B |         - |           ? |
| Sub_Alias       | Sub        | 0.3278 ns | 0.2392 ns | 0.0131 ns | 0.3258 ns |     ? |       ? |      41 B |         - |           ? |
