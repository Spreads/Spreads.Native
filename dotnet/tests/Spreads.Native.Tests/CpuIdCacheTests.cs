using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public class ProcessorIdCacheTests
    {
        [Test]
        public void CouldGetCpuNumberFromCache()
        {
            var nativeCpuId = CpuIdCache.get_cpu_number();
            Console.WriteLine($"native: {nativeCpuId}");

            var cpuId = CpuIdCache.GetCurrentCpuId();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.IsTrue(nativeCpuId >= 0);
            }

            Assert.IsTrue(cpuId >= 0);
            Console.WriteLine($"cached: {cpuId}");
        }

        [Test, Explicit("Bench")]
        public void CpuNumberPerformance()
        {
            var count = 100_000_000;

            for (int r = 0; r < 20; r++)
            {
                using (Benchmark.Run("dotnet", count))
                {
                    GetCupuNumberDotnet(count);
                }

                using (Benchmark.Run("spreads", count))
                {
                    GetCupuNumberSpreads(count);
                }
            }

            Benchmark.Dump();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GetCupuNumberDotnet(int count)
        {
            var x = 0L;
            for (int i = 0; i < count; i++)
            {
                x += Thread.GetCurrentProcessorId();
            }

            Assert.IsTrue(x > 0, "dotnet" + x);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
        private void GetCupuNumberSpreads(int count)
        {
            var x = 0L;
            for (int i = 0; i < count; i++)
            {
                x += CpuIdCache.GetCurrentCpuId();
                // CpuIdCache.FlushCurrentCpuId();
            }

            Assert.IsTrue(x >= 0, "Spreads: " + x);
        }
    }
}