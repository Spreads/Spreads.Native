using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
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
            var nativeCpuId = Cpu.get_cpu_number();
            Console.WriteLine($"native: {nativeCpuId}");

            var cpuId = Cpu.GetCurrentCoreId();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.IsTrue(nativeCpuId >= 0);
            }

            Assert.IsTrue(cpuId >= 0);
            Console.WriteLine($"cached: {cpuId}");
        }

#if NETCOREAPP
        [Test, Explicit("Bench")]
        public void CpuNumberPerformance()
        {
            var count = 10_000_000;

            for (int r = 0; r < 10; r++)
            {
                using (Benchmark.Run(".NET Core", count, true))
                {
                    GetCupuNumberDotnet(count);
                }

                using (Benchmark.Run("Spreads.Native", count, true))
                {
                    GetCupuNumberSpreads(count);
                }

                using (Benchmark.Run("Spreads.Pal", count, true))
                {
                    GetCupuNumberSpreadsPal(count);
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
                x += Cpu.GetCurrentCoreId();
                // Cpu.FlushCurrentCpuId();
            }

            Assert.IsTrue(x >= 0, "Spreads: " + x);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
        private void GetCupuNumberSpreadsPal(int count)
        {
            var x = 0L;
            for (int i = 0; i < count; i++)
            {
                x += Pal.GetCurrentCoreId();
                // Pal.FlushCurrentCpuId();
            }

            Assert.IsTrue(x >= 0, "Spreads: " + x);
        }

        /// <summary>
        /// Platform abstraction layer
        /// </summary>
        public static class Pal
        {
            private static readonly int _coreCount = Environment.ProcessorCount;

            private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            private static readonly bool _schedGetCpuWorks = TestSchedGetCpu();

            private static bool TestSchedGetCpu()
            {
                if (_isWindows) return false;
                try
                {
                    if (sched_getcpu() >= 0)
                        return true;
                }
                catch(Exception ex)
                {
                    // ignored
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static int get_cpu_number()
            {
                if (_isWindows)
                {
                    if (_coreCount > 64)
                    {
                        ProcessorNumber procNum = default;
                        GetCurrentProcessorNumberEx(ref procNum);
                        return (procNum.Group << 6) | procNum.Number;
                    }

                    return (int)GetCurrentProcessorNumber();
                }

                if (_schedGetCpuWorks)
                    return sched_getcpu();

                return -1;
            }

            [SuppressUnmanagedCodeSecurity, SuppressGCTransition]
            [DllImport("kernel32.dll")]
            private static extern uint GetCurrentProcessorNumber();

#pragma warning disable 649
            /// <summary>
            /// Represents a logical processor in a processor group.
            /// </summary>
            [SuppressMessage("ReSharper", "UnassignedField.Global")]
            private struct ProcessorNumber
            {
                /// <summary>
                /// The processor group to which the logical processor is assigned.
                /// </summary>
                public ushort Group;

                /// <summary>
                /// The number of the logical processor relative to the group.
                /// </summary>
                public byte Number;

                /// <summary>
                /// This parameter is reserved.
                /// </summary>
                public byte Reserved;
            }

#pragma warning restore 649

            [SuppressUnmanagedCodeSecurity, SuppressGCTransition]
            [DllImport("kernel32.dll")]
            private static extern void GetCurrentProcessorNumberEx(ref ProcessorNumber processorNumber);

            [SuppressUnmanagedCodeSecurity, SuppressGCTransition]
            [DllImport("libc.so.6", SetLastError = true)]
            public static extern int sched_getcpu();

            /// <summary>
            /// The number of cpu cores available for the current process, capped at <see cref="MaxCores"/>.
            /// </summary>
            public static readonly int CoreCount = Environment.ProcessorCount;

            // The upper bits of t_currentProcessorIdCache are the currentProcessorId. The lower bits of
            // the t_currentProcessorIdCache are counting down to get it periodically refreshed.
            [ThreadStatic]
            private static int _currentProcessorIdCache;

            private const int CacheShift = 16;

            private const int CacheCountDownMask = (1 << CacheShift) - 1;

            private const int RefreshRate = 100;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int RefreshCurrentCoreId()
            {
                int currentProcessorId = Pal.get_cpu_number();

                // On Unix, GetCurrentProcessorNumber() is implemented in terms of sched_getcpu, which
                // doesn't exist on all platforms.  On those it doesn't exist on, GetCurrentProcessorNumber()
                // returns -1.  As a fallback in that case and to spread the threads across the buckets
                // by default, we use the current managed thread ID as a proxy.
                if (currentProcessorId < 0)
                    currentProcessorId = Environment.CurrentManagedThreadId;

                // Make CPU id a valid index from [0, CoreCount)
                if (currentProcessorId >= CoreCount)
                    currentProcessorId %= CoreCount;

                // Mask with int.MaxValue to ensure the execution Id is not negative
                _currentProcessorIdCache = ((currentProcessorId << CacheShift) & int.MaxValue) |
                                           RefreshRate;

                return currentProcessorId;
            }

            /// <summary>
            /// Returns a cached id of the current core. The value is always
            /// valid as an index of an array with a length of <see cref="CoreCount"/>
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetCurrentCoreId()
            {
                int currentProcessorIdCache = _currentProcessorIdCache--;
                if ((currentProcessorIdCache & CacheCountDownMask) == 0)
                {
                    return RefreshCurrentCoreId();
                }

                return currentProcessorIdCache >> CacheShift;
            }

            /// <summary>
            /// Consider flushing the currentProcessorIdCache on Wait operations or similar
            /// actions that are likely to result in changing the executing core.
            /// </summary>
            public static void FlushCurrentCpuId()
            {
                _currentProcessorIdCache &= (~CacheCountDownMask) | 1;
            }
        }
#endif
    }
}
