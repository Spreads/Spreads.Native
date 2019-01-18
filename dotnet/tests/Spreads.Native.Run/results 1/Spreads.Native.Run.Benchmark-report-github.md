``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.0.100-preview-010046
  [Host]             : .NET Core 2.1.7 (CoreCLR 4.6.27129.04, CoreFX 4.6.27129.04), 64bit RyuJIT
  .NET 4.6.1         : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3190.0
  .NET Core 2.1 LTS  : .NET Core 2.1.7 (CoreCLR 4.6.27129.04, CoreFX 4.6.27129.04), 64bit RyuJIT
  .NET Core 3.0.1-pv : .NET Core 3.0.0-preview-27308-3 (CoreCLR 4.6.27220.01, CoreFX 4.7.19.5401), 64bit RyuJIT

IterationCount=100  LaunchCount=3  WarmupCount=15  

```
|                 Method |                Job |     Toolchain |      Mean |     Error |    StdDev |    Median |            Op/s |
|----------------------- |------------------- |-------------- |----------:|----------:|----------:|----------:|----------------:|
| Vec&lt;T&gt;.GetUnchecked(i) |         .NET 4.6.1 |        net461 | 0.7042 ns | 0.0003 ns | 0.0014 ns | 0.7039 ns | 1,420,090,626.1 |
| Vec.GetUnchecked&lt;T&gt;(i) |         .NET 4.6.1 |        net461 | 0.7060 ns | 0.0004 ns | 0.0021 ns | 0.7067 ns | 1,416,486,142.0 |
| (T)Vec.GetUnchecked(i) |         .NET 4.6.1 |        net461 | 5.0194 ns | 0.0027 ns | 0.0136 ns | 5.0160 ns |   199,226,814.9 |
|                 &#39;T[i]&#39; |         .NET 4.6.1 |        net461 | 0.7057 ns | 0.0003 ns | 0.0015 ns | 0.7056 ns | 1,417,117,527.2 |
|           &#39;Span&lt;T&gt;[i]&#39; |         .NET 4.6.1 |        net461 | 1.0150 ns | 0.0018 ns | 0.0093 ns | 1.0161 ns |   985,236,693.9 |
|    &#39;Memory&lt;T&gt;.Span[i]&#39; |         .NET 4.6.1 |        net461 | 4.4849 ns | 0.0023 ns | 0.0117 ns | 4.4807 ns |   222,971,397.6 |
| Vec&lt;T&gt;.GetUnchecked(i) |  .NET Core 2.1 LTS | .NET Core 2.1 | 0.7029 ns | 0.0001 ns | 0.0004 ns | 0.7028 ns | 1,422,599,266.0 |
| Vec.GetUnchecked&lt;T&gt;(i) |  .NET Core 2.1 LTS | .NET Core 2.1 | 0.7038 ns | 0.0003 ns | 0.0018 ns | 0.7029 ns | 1,420,777,170.3 |
| (T)Vec.GetUnchecked(i) |  .NET Core 2.1 LTS | .NET Core 2.1 | 5.6106 ns | 0.0054 ns | 0.0271 ns | 5.5982 ns |   178,234,936.3 |
|                 &#39;T[i]&#39; |  .NET Core 2.1 LTS | .NET Core 2.1 | 0.8521 ns | 0.0008 ns | 0.0042 ns | 0.8532 ns | 1,173,525,943.3 |
|           &#39;Span&lt;T&gt;[i]&#39; |  .NET Core 2.1 LTS | .NET Core 2.1 | 0.6739 ns | 0.0030 ns | 0.0157 ns | 0.6769 ns | 1,483,922,377.4 |
|    &#39;Memory&lt;T&gt;.Span[i]&#39; |  .NET Core 2.1 LTS | .NET Core 2.1 | 3.7598 ns | 0.0018 ns | 0.0094 ns | 3.7615 ns |   265,969,219.5 |
| Vec&lt;T&gt;.GetUnchecked(i) | .NET Core 3.0.1-pv | .NET Core 3.0 | 0.7038 ns | 0.0002 ns | 0.0012 ns | 0.7034 ns | 1,420,946,639.1 |
| Vec.GetUnchecked&lt;T&gt;(i) | .NET Core 3.0.1-pv | .NET Core 3.0 | 0.7044 ns | 0.0004 ns | 0.0021 ns | 0.7034 ns | 1,419,642,748.6 |
| (T)Vec.GetUnchecked(i) | .NET Core 3.0.1-pv | .NET Core 3.0 | 5.1410 ns | 0.0101 ns | 0.0511 ns | 5.1265 ns |   194,515,362.6 |
|                 &#39;T[i]&#39; | .NET Core 3.0.1-pv | .NET Core 3.0 | 0.8479 ns | 0.0010 ns | 0.0051 ns | 0.8469 ns | 1,179,448,404.5 |
|           &#39;Span&lt;T&gt;[i]&#39; | .NET Core 3.0.1-pv | .NET Core 3.0 | 0.8593 ns | 0.0004 ns | 0.0018 ns | 0.8584 ns | 1,163,803,005.6 |
|    &#39;Memory&lt;T&gt;.Span[i]&#39; | .NET Core 3.0.1-pv | .NET Core 3.0 | 2.1072 ns | 0.0002 ns | 0.0012 ns | 2.1070 ns |   474,566,552.2 |
