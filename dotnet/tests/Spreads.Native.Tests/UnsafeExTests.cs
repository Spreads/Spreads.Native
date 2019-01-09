// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

// ReSharper disable PossibleNullReferenceException

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public unsafe class UnsafeExTests
    {
        public static readonly int ElemOffset = UnsafeEx.ElemOffset(new int[1]);
        public static readonly int ElemSize = Unsafe.SizeOf<int>();

        public static class Helper<T>
        {
            public static readonly int ElemOffset = UnsafeEx.ElemOffset(new T[1]);
            public static readonly int ElemSize = Unsafe.SizeOf<T>();
        }

        [Test]
        public void UnsafeExWorks()
        {
            var arr = new int[] { 1, 2, 3 };
            var offset = UnsafeEx.ElemOffset(arr);
            Console.WriteLine(offset);

            var snd = UnsafeEx.Get<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(2, snd);

            UnsafeEx.Set(arr, (byte*)(offset + 4), 42);

            snd = UnsafeEx.Get<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(42, snd);
        }

        [Test, Explicit("Long running")]
        public void GetSetBench()
        {
            var arr = new int[] { 1, 2, 3 };

            var count = 100_000_000;
            var rounds = 10;

            for (int r = 0; r < rounds; r++)
            {
                long sumArr = 0;
                using (Benchmark.Run("Arr", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumArr += arr[2];
                    }
                }

                long sumUnsafe = 0;
                using (Benchmark.Run("Unsafe", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumUnsafe += UnsafeEx.Get<int>(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize));
                    }
                }

                Assert.AreEqual(sumArr, sumUnsafe);

                UnsafeEx.Set(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize), arr[2]++);
            }

            Benchmark.Dump();
        }
    }
}