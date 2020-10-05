using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Spreads.Native.Run
{
    [MemoryDiagnoser]
    [CategoriesColumn, RankColumn]
    [GroupBenchmarksBy(new[] {BenchmarkLogicalGroupRule.ByMethod})]
    [MarkdownExporterAttribute.GitHub]
    [DisassemblyDiagnoser]
    public class CpuIdBench
    {
        [Benchmark(OperationsPerInvoke = 1)]
        public void GetCpuId()
        {
            Cpu.GetCurrentCoreId();
        }
    }
}