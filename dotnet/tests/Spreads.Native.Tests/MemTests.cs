using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using HdrHistogram;
using NUnit.Framework;

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public unsafe class MemTests
    {

        // TODO Uncomment when NuGet is updated
        // [Test]
        // public void CouldGetProcessInfo()
        // {
        //     var info = Mem.ProcessInfo();
        //     Console.WriteLine(info);
        // }

        // [Test]
        // public void CouldGetVersion()
        // {
        //     Assert.IsTrue(Mem.MimallocVersion() >= 200);
        // }

        [Test]
        public void CouldAllocFreeFromDifferentThreads()
        {
            var bc = new BlockingCollection<IntPtr>();
            var are = new AutoResetEvent(false);
            var count = 100;
            var t1 = new Thread(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    var ptr = Mem.MallocAligned((UIntPtr) (2 * 1024 * 1024 + 1), (UIntPtr) (8));
                    bc.Add((IntPtr) ptr);
                    are.WaitOne();
                }

                bc.CompleteAdding();
            });
            t1.Start();

            var t2 = new Thread(() =>
            {
                foreach (var intPtr in bc.GetConsumingEnumerable())
                {
                    Mem.Free((byte*) intPtr);
                    are.Set();
                }
            });
            t2.Start();
            t1.Join();
            t2.Join();
        }

        [Test, Explicit]
        public void CouldFreeOnDifferentThread()
        {
            Mem.RegisterOutput((str, arg) => { Console.Write(str); }, null);

            var length = 2 * 1024 * 1024 + 1 ; // fails > 2MB
            for (int i = 0; i < 1; i++)
            {
                var pm = Mem.Malloc((UIntPtr) (length));
                // Mem.Free(pm); // works
                Task.Run(() =>
                {
                    Mem.Free(pm); // fails
                }).Wait();
            }
        }

        [Test]
        public void CouldAllocFree()
        {
            var p = Mem.Malloc((UIntPtr) 123);
            p = Mem.Realloc(p, (UIntPtr) 456);
            p = Mem.Reallocf(p, (UIntPtr) 789);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Calloc((UIntPtr) 1, (UIntPtr) 2);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.Recalloc(p, (UIntPtr) 10, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.Expand(p, (UIntPtr) Mem.UsableSize(p));
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.Collect(false);

            p = Mem.MallocSmall((UIntPtr) 42);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocSmall((UIntPtr) 42);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Zalloc((UIntPtr) 42000);
            Assert.IsTrue(p != null);
            p = Mem.Rezalloc(p, (UIntPtr) 100000);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Mallocn((UIntPtr) 10, (UIntPtr) 20);
            Assert.IsTrue(p != null);
            p = Mem.Reallocn(p, (UIntPtr) 100, (UIntPtr) 20);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.Collect(true);

            p = Mem.MallocAligned((UIntPtr) (16 * 17), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.ReallocAligned(p, (UIntPtr) (16 * 30), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.MallocAlignedAt((UIntPtr) (3 + 16 * 17), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.ReallocAlignedAt(p, (UIntPtr) (3 + 16 * 34), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocAligned((UIntPtr) (16 * 17), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            p = Mem.RezallocAligned(p, (UIntPtr) (16 * 30), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocAlignedAt((UIntPtr) (3 + 16 * 17), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.RezallocAlignedAt(p, (UIntPtr) (3 + 16 * 34), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.CallocAligned((UIntPtr) 17, (UIntPtr) 16, (UIntPtr) 16);
            Assert.IsTrue(p != null);
            p = Mem.RecallocAligned(p, (UIntPtr) 34, (UIntPtr) 16, (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.CallocAlignedAt((UIntPtr) 17, (UIntPtr) 16, (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.RecallocAlignedAt(p, (UIntPtr) 34, (UIntPtr) 16, (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);
        }

        [Test]
        public void CouldAllocFreeHeap()
        {
            var h = Mem.HeapNew();

            var p = Mem.HeapMalloc(h, (UIntPtr) 123);
            p = Mem.HeapRealloc(h, p, (UIntPtr) 456);
            p = Mem.HeapReallocf(h, p, (UIntPtr) 789);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapCalloc(h, (UIntPtr) 1, (UIntPtr) 2);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.HeapRecalloc(h, p, (UIntPtr) 10, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.Expand(p, (UIntPtr) Mem.UsableSize(p));
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.HeapCollect(h, false);

            p = Mem.HeapMallocSmall(h, (UIntPtr) 42);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapZalloc(h, (UIntPtr) 42000);
            Assert.IsTrue(p != null);
            p = Mem.HeapRezalloc(h, p, (UIntPtr) 100000);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapMallocn(h, (UIntPtr) 10, (UIntPtr) 20);
            Assert.IsTrue(p != null);
            p = Mem.HeapReallocn(h, p, (UIntPtr) 100, (UIntPtr) 20);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.HeapCollect(h, true);

            p = Mem.HeapMallocAligned(h, (UIntPtr) (16 * 17), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.HeapReallocAligned(h, p, (UIntPtr) (16 * 30), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapMallocAlignedAt(h, (UIntPtr) (3 + 16 * 17), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.HeapReallocAlignedAt(h, p, (UIntPtr) (3 + 16 * 34), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapZallocAligned(h, (UIntPtr) (16 * 17), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            p = Mem.HeapRezallocAligned(h, p, (UIntPtr) (16 * 30), (UIntPtr) 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapZallocAlignedAt(h, (UIntPtr) (3 + 16 * 17), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.HeapRezallocAlignedAt(h, p, (UIntPtr) (3 + 16 * 34), (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapCallocAligned(h, (UIntPtr) 10, (UIntPtr) 16, (UIntPtr) 8);
            Assert.IsTrue(p != null);
            p = Mem.HeapRecallocAligned(h, p, (UIntPtr) 20, (UIntPtr) 16, (UIntPtr) 8);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.HeapCallocAlignedAt(h, (UIntPtr) 17, (UIntPtr) 16, (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            p = Mem.HeapRecallocAlignedAt(h, p, (UIntPtr) 34, (UIntPtr) 16, (UIntPtr) 16, (UIntPtr) 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.HeapDestroy(h);
        }

        [Test, Explicit]
        public void MimallocAllocFreePerf()
        {
            var h = Mem.HeapNew();

            Mem.OptionSetEnabled(Mem.Option.EagerCommit, true);
            Mem.OptionSetEnabled(Mem.Option.LargeOsPages, true);
            Mem.OptionSetEnabled(Mem.Option.ResetDecommits, true);
            Mem.OptionSetEnabled(Mem.Option.PageReset, true);
            Mem.OptionSetEnabled(Mem.Option.SegmentReset, true);
            Mem.OptionSetEnabled(Mem.Option.AbandonedPageReset, true);
            Mem.OptionSet(Mem.Option.ResetDelay, 0);
            Mem.OptionSetEnabled(Mem.Option.EagerRegionCommit, true);

            Mem.RegisterOutput((str, arg) => { Console.Write(str); }, null);

            Assert.IsTrue(Mem.OptionIsEnabled(Mem.Option.PageReset));

            var count = 100_000L;
            var size = 32 * 4096;
            IntPtr[] ptrs = new IntPtr[count];

            for (int r = 0; r < 4; r++)
            {
                Task.Factory.StartNew(() =>
                    {
                        using (Benchmark.Run("Alloc" + r, (long) (count * size / 1000.0)))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                ptrs[i] = (IntPtr) Mem.HeapMalloc(h, (UIntPtr) size);
                                if ((long) (ptrs[i]) % 4096 != 0)
                                {
                                    Assert.Fail(((long) (ptrs[i]) % 4096).ToString());
                                    // Console.WriteLine((long)(ptrs[i]) % 4096);
                                }

                                for (int j = 0; j < size; j += 4096) ((byte*) ptrs[i])[j] = 0;
                                ((byte*) ptrs[i])[size - 1] = 0;
                            }
                        }
                    }
                    , TaskCreationOptions.LongRunning).Wait();

                Task.Factory.StartNew(() =>
                    {
                        using (Benchmark.Run("Free" + r, (long) (count * size / 1000.0)))
                        {
                            for (long i = count - 1; i >= 0; i--)
                            {
                                Mem.Free((byte*) ptrs[i]);
                            }

                            // Mem.HeapCollect(h, true);
                        }
                    }
                    , TaskCreationOptions.LongRunning).Wait();
            }

            Mem.StatsPrint();

            // long x = 0;
            // while (true)
            // {
            //     x++;
            //
            //     ptrs[0] = (IntPtr) Spreads.Native.Mem.Malloc((UIntPtr) size);
            //     Mem.Free((byte*) ptrs[0]);
            //     if (x == long.MaxValue)
            //         break;
            //
            //     Thread.Sleep(1);
            // }
            //
            // // Spreads.Native.Mem.Collect(false);
            //
            //
            // Thread.Sleep(10000000);
        }

        [Test, Explicit]
        public void MimallocAllocFreeCallPerf()
        {
            Mem.OptionSetEnabled(Mem.Option.EagerCommit, true);
            Mem.OptionSetEnabled(Mem.Option.LargeOsPages, true);
            Mem.OptionSetEnabled(Mem.Option.ResetDecommits, true);
            Mem.OptionSetEnabled(Mem.Option.PageReset, true);
            Mem.OptionSetEnabled(Mem.Option.SegmentReset, true);
            Mem.OptionSetEnabled(Mem.Option.AbandonedPageReset, true);
            // Mem.OptionSet(Mem.Option.ResetDelay, 0);
            Mem.OptionSetEnabled(Mem.Option.EagerRegionCommit, true);

            var h = Mem.HeapNew();

            var count = 100_000L;
            var size = 32 * 4096;
            IntPtr[] ptrs = new IntPtr[count];

            for (int r = 0; r < 4; r++)
            {
                using (Benchmark.Run("AllocFree" + r, count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        ptrs[i] = (IntPtr) Mem.HeapMalloc(h, (UIntPtr) size);
                        for (int j = 0; j < size; j += 4096) ((byte*) ptrs[i])[j] = 0;
                        ((byte*) ptrs[i])[size - 1] = 0;
                        Mem.Free((byte*) ptrs[i]);
                    }
                }
            }

            Mem.StatsPrint();
        }

        [Test, Explicit]
        public void MimallocAllocFreeCallLatency()
        {
            Mem.OptionSetEnabled(Mem.Option.EagerCommit, true);
            Mem.OptionSetEnabled(Mem.Option.LargeOsPages, true);
            Mem.OptionSetEnabled(Mem.Option.ResetDecommits, true);
            Mem.OptionSetEnabled(Mem.Option.PageReset, true);
            Mem.OptionSetEnabled(Mem.Option.SegmentReset, true);
            Mem.OptionSetEnabled(Mem.Option.AbandonedPageReset, true);
            // Mem.OptionSet(Mem.Option.ResetDelay, 0);
            Mem.OptionSetEnabled(Mem.Option.EagerRegionCommit, true);

            Mem.RegisterOutput((str, arg) => { Console.Write(str); }, null);

            var rng = new Random();
            var allocated = 0L;

            var h = Mem.HeapNew();

            var count = 100_000L;
            var size = 32 * 4096;
            IntPtr[] ptrs = new IntPtr[count];

            for (int r = 0; r < 4; r++)
            {
                var histogram = new HdrHistogram.LongHistogram(1, 1000000, 1);

                using (Benchmark.Run("AllocFree" + r, count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var x = rng.NextDouble();
                        var start = Stopwatch.GetTimestamp();
                        if (allocated < 1000)
                            Allocate();
                        else if (allocated > 2000)
                            Free();
                        else if (((2000 - allocated) / 1000.0 * x) > 0.5)
                            Allocate();
                        else
                            Free();

                        var time = Stopwatch.GetTimestamp() - start;

                        histogram.RecordValue(time);

                        void Allocate()
                        {
                            ptrs[allocated] = (IntPtr) Mem.HeapMalloc(h, (UIntPtr) size);
                            for (int j = 0; j < size; j += 4096) ((byte*) ptrs[allocated])[j] = 0;
                            ((byte*) ptrs[allocated])[size - 1] = 0;
                            allocated++;
                        }

                        void Free()
                        {
                            Mem.Free((byte*) ptrs[allocated - 1]);
                            allocated--;
                        }
                    }
                }

                histogram.OutputPercentileDistribution(Console.Out,
                    outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
            }

            Mem.StatsPrint();
        }

        [Test, Explicit]
        public unsafe void MarshalAllocFreePerf()
        {
            var count = 100_000L;
            var size = 32 * 4096;
            IntPtr[] ptrs = new IntPtr[count];

            for (int r = 0; r < 4; r++)
            {
                using (Benchmark.Run("Alloc" + r, (long) (count * size / 1000.0)))
                {
                    for (int i = 0; i < count; i++)
                    {
                        ptrs[i] = Marshal.AllocHGlobal(size);

                        // It's unaligned
                        // if ((long)(ptrs[i]) % 4096 != 0)
                        //     Assert.Fail(((long) (ptrs[i]) % 4096).ToString());

                        for (int j = 0; j < size; j += 4096) ((byte*) ptrs[i])[j] = 0;
                        ((byte*) ptrs[i])[size - 1] = 0;
                    }
                }

                using (Benchmark.Run("Free" + r, (long) (count * size / 1000.0)))
                {
                    for (int i = 0; i < count; i++)
                    {
                        Marshal.FreeHGlobal(ptrs[i]);
                    }
                }
            }
        }

        [Test, Explicit]
        public unsafe void VirtualAllocFreePerf()
        {
            var count = 100_000L;
            var size = 16 * 4096;
            IntPtr[] ptrs = new IntPtr[count];

            for (int r = 0; r < 3; r++)
            {
                using (Benchmark.Run("Alloc" + r, (long) (count * size / 1000.0)))
                {
                    for (int i = 0; i < count; i++)
                    {
                        ptrs[i] = Kernel32.VirtualAlloc(IntPtr.Zero, (uint) size,
                            Kernel32.Consts.MEM_COMMIT | Kernel32.Consts.MEM_RESERVE, Kernel32.Consts.PAGE_READWRITE);

                        if ((long) (ptrs[i]) % 4096 != 0)
                            Assert.Fail(((long) (ptrs[i]) % 4096).ToString());

                        for (int j = 0; j < size; j += 4096) ((byte*) ptrs[i])[j] = 0;
                        ((byte*) ptrs[i])[size - 1] = 0;
                    }
                }

                using (Benchmark.Run("Free" + r, (long) (count * size / 1000.0)))
                {
                    for (int i = 0; i < count; i++)
                    {
                        Kernel32.VirtualFree(ptrs[i], 0, Kernel32.Consts.MEM_RELEASE);
                    }
                }
            }
        }

        public static class Kernel32
        {
            [DllImport("Kernel32", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            internal static extern IntPtr VirtualAlloc([In] IntPtr lpAddress, [In] uint dwSize,
                [In] int flAllocationType, [In] int flProtect);

            [DllImport("Kernel32", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            internal static extern bool VirtualFree([In] IntPtr lpAddress, [In] uint dwSize, [In] int dwFreeType);

            public static class Consts
            {
                public const int MEM_COMMIT = 0x00001000;
                public const int MEM_RESERVE = 0x00002000;
                public const int MEM_RELEASE = 0x8000;
                public const int PAGE_READWRITE = 0x04;
            }
        }
    }
}
