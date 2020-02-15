using System;
using NUnit.Framework;

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public unsafe class MemTests
    {
        [Test]
        public void CouldGetMimallocVersion()
        {
            var version = Mem.MimallocVersion();
            Assert.AreEqual(160, version);
            Console.WriteLine($"Version: {version}");
        }

        [Test]
        public void CouldAllocFree()
        {
            var p = Mem.Malloc(123);
            p = Mem.Realloc(p, 456);
            p = Mem.Reallocf(p, 789);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Calloc(1, 2);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.Recalloc(p, 10, 3);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.Expand(p, (uint) Mem.UsableSize(p));
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.Collect(false);

            p = Mem.MallocSmall(42);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocSmall(42);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Zalloc(42000);
            Assert.IsTrue(p != null);
            p = Mem.Rezalloc(p, 100000);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.Mallocn(10, 20);
            Assert.IsTrue(p != null);
            p = Mem.Reallocn(p, 100, 20);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            Mem.Collect(true);

            p = Mem.MallocAligned(16 * 17, 16);
            Assert.IsTrue(p != null);
            Console.WriteLine(Mem.UsableSize(p));
            p = Mem.ReallocAligned(p, 16 * 30, 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.MallocAlignedAt(3 + 16 * 17, 16, 3);
            Assert.IsTrue(p != null);
            p = Mem.ReallocAlignedAt(p, 3 + 16 * 34, 16, 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocAligned(16 * 17, 16);
            Assert.IsTrue(p != null);
            p = Mem.RezallocAligned(p, 16 * 30, 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.ZallocAlignedAt(3 + 16 * 17, 16, 3);
            Assert.IsTrue(p != null);
            p = Mem.RezallocAlignedAt(p, 3 + 16 * 34, 16, 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.CallocAligned(17, 16, 16);
            Assert.IsTrue(p != null);
            p = Mem.RecallocAligned(p, 34, 16, 16);
            Assert.IsTrue(p != null);
            Mem.Free(p);

            p = Mem.CallocAlignedAt(17, 16, 16, 3);
            Assert.IsTrue(p != null);
            p = Mem.RecallocAlignedAt(p, 34, 16, 16, 3);
            Assert.IsTrue(p != null);
            Mem.Free(p);
        }
    }
}