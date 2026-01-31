## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Add_Raw()
       vmovups   xmm0,[rdi+0C]
       vaddps    xmm0,xmm0,[rdi+1C]
       vmovhlps  xmm1,xmm1,xmm0
       ret
; Total bytes of code 15
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Add_Alias()
       sub       rsp,18
       vmovups   xmm0,[rdi+2C]
       vmovups   xmm1,[rdi+3C]
       vaddps    xmm0,xmm0,xmm1
       vmovups   [rsp+8],xmm0
       vmovsd    xmm0,qword ptr [rsp+8]
       vmovsd    xmm1,qword ptr [rsp+10]
       add       rsp,18
       ret
; Total bytes of code 41
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Eq_Raw()
       vmovups   xmm0,[rdi+0C]
       vcmpeqps  xmm0,xmm0,[rdi+1C]
       vmovmskps eax,xmm0
       cmp       eax,0F
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 25
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Eq_Alias()
       vmovups   xmm0,[rdi+2C]
       vmovups   xmm1,[rdi+3C]
       vcmpeqps  xmm0,xmm0,xmm1
       vmovmskps eax,xmm0
       cmp       eax,0F
       sete      al
       movzx     eax,al
       ret
; Total bytes of code 29
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Neg_Raw()
       vmovups   xmm0,[rdi+0C]
       vxorps    xmm0,xmm0,[7F617BCCA420]
       vmovhlps  xmm1,xmm1,xmm0
       ret
; Total bytes of code 18
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Neg_Alias()
       sub       rsp,18
       vmovups   xmm0,[rdi+2C]
       vxorps    xmm0,xmm0,[7FE99C6EA460]
       vmovups   [rsp+8],xmm0
       vmovsd    xmm0,qword ptr [rsp+8]
       vmovsd    xmm1,qword ptr [rsp+10]
       add       rsp,18
       ret
; Total bytes of code 40
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.ScalarMul_Raw()
       vmovups   xmm0,[rdi+0C]
       vbroadcastss xmm1,dword ptr [rdi+8]
       vmulps    xmm0,xmm1,xmm0
       vmovhlps  xmm1,xmm1,xmm0
       ret
; Total bytes of code 20
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.ScalarMul_Alias()
       sub       rsp,18
       vmovups   xmm0,[rdi+2C]
       vbroadcastss xmm1,dword ptr [rdi+8]
       vmulps    xmm0,xmm1,xmm0
       vmovups   [rsp+8],xmm0
       vmovsd    xmm0,qword ptr [rsp+8]
       vmovsd    xmm1,qword ptr [rsp+10]
       add       rsp,18
       ret
; Total bytes of code 42
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Sub_Raw()
       vmovups   xmm0,[rdi+0C]
       vsubps    xmm0,xmm0,[rdi+1C]
       vmovhlps  xmm1,xmm1,xmm0
       ret
; Total bytes of code 15
```

## .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; newtype.benchmark.Vector4ArithmeticBenchmarks.Sub_Alias()
       sub       rsp,18
       vmovups   xmm0,[rdi+2C]
       vmovups   xmm1,[rdi+3C]
       vsubps    xmm0,xmm0,xmm1
       vmovups   [rsp+8],xmm0
       vmovsd    xmm0,qword ptr [rsp+8]
       vmovsd    xmm1,qword ptr [rsp+10]
       add       rsp,18
       ret
; Total bytes of code 41
```

