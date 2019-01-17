// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using NUnit.Framework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

            snd = UnsafeEx.GetRef<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(42, snd);
        }

        [Test]
        public void UnsafeExWorksWithStrings()
        {
            var arr = new string[] { "1", "2", "3" };

            var offset = UnsafeEx.ElemOffset(arr);
            Console.WriteLine("Offset: " + offset);
            Console.WriteLine("Element size: " + Helper<string>.ElemSize);

            var snd = UnsafeEx.Get<string>(arr, (byte*)(offset + Helper<string>.ElemSize));
            Assert.AreEqual("2", snd);

            UnsafeEx.Set(arr, (byte*)(offset + Helper<string>.ElemSize), "42");

            snd = UnsafeEx.GetRef<string>(arr, (byte*)(offset + Helper<string>.ElemSize));
            Assert.AreEqual("42", snd);
        }

        [Test]
        public void UnsafeExWorksViaPinnedPtr()
        {
            var arr = new int[] { 1, 2, 3 };
            var handle = ((Memory<int>)arr).Pin();
            var ptr = (byte*)handle.Pointer;

            var offset = UnsafeEx.ElemOffset(arr);
            Console.WriteLine(offset);

            var snd = UnsafeEx.Get<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(2, snd);

            UnsafeEx.Set(arr, (byte*)(offset + 4), 42);

            snd = UnsafeEx.GetRef<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(42, snd);

            snd = UnsafeEx.GetRef<int>(null, (byte*)(ptr + 4));
            Assert.AreEqual(42, snd);
        }

        [Test]
        public void UnsafeExWorksWithNull()
        {
            var ptr = (byte*)Marshal.AllocHGlobal(64);

            int[] arr = null;

            *(((int*)ptr) + 1) = 2;

            var snd = UnsafeEx.Get<int>(arr, (byte*)(ptr + 4));
            Assert.AreEqual(2, snd);

            UnsafeEx.Set(arr, (byte*)(ptr + 4), 42);

            snd = UnsafeEx.GetRef<int>(arr, (byte*)(ptr + 4));
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
                using (Benchmark.Run("Arr GET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumArr += arr[2];
                    }
                }

                long sumUnsafe = 0;
                using (Benchmark.Run("UnsafeEx GET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumUnsafe += UnsafeEx.GetRef<int>(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize));
                    }
                }

                long sumArrX = 0;
                using (Benchmark.Run("Arr SET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        arr[2] = i;
                        sumArrX++;
                    }
                }

                long sumUnsafeX = 0;
                using (Benchmark.Run("UnsafeEx SET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        UnsafeEx.GetRef<int>(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize)) = i;
                        sumUnsafeX++;
                    }
                }

                Assert.AreEqual(sumArr, sumUnsafe);
                Assert.AreEqual(sumArrX, sumUnsafeX);

                UnsafeEx.Set(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize), arr[2]++);
            }

            Benchmark.Dump();
        }

        [Test, Explicit("Long running")]
        public void GetSetBenchPinned()
        {
            var arr = new int[] { 1, 2, 3 };
            var handle = ((Memory<int>)arr).Pin();
            var ptr = (byte*)handle.Pointer;

            var count = 100_000_000;
            var rounds = 10;

            for (int r = 0; r < rounds; r++)
            {
                long sumArr = 0;
                using (Benchmark.Run("Arr GET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumArr += arr[2];
                    }
                }

                long sumUnsafe = 0;
                using (Benchmark.Run("UnsafeEx GET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumUnsafe += UnsafeEx.GetRef<int>(null, (byte*)(ptr + 2 * Helper<int>.ElemSize));
                    }
                }

                long sumArrX = 0;
                using (Benchmark.Run("Arr SET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        arr[2] = i;
                        sumArrX++;
                    }
                }

                long sumUnsafeX = 0;
                using (Benchmark.Run("UnsafeEx SET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        UnsafeEx.GetRef<int>(null, (byte*)(ptr + 2 * Helper<int>.ElemSize)) = i;
                        sumUnsafeX++;
                    }
                }

                Assert.AreEqual(sumArr, sumUnsafe);
                Assert.AreEqual(sumArrX, sumUnsafeX);

                UnsafeEx.Set(arr, (byte*)(Helper<int>.ElemOffset + 2 * Helper<int>.ElemSize), arr[2]++);
            }

            Benchmark.Dump();
        }

        [Test, Explicit("Long running")]
        public void GetSetBenchWithNull()
        {
            int[] arr = null;

            var arr2 = new int[] { 1, 2, 3 };

            var ptr = (byte*)Marshal.AllocHGlobal(64);

            var count = 100_000_000;
            var rounds = 10;

            for (int r = 0; r < rounds; r++)
            {
                long sumUnsafe = 0;
                using (Benchmark.Run("UnsafeEx GET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sumUnsafe += UnsafeEx.Get<int>(arr, (byte*)(ptr + 2 * Helper<int>.ElemSize));
                    }
                }

                long sumUnsafeX = 0;
                using (Benchmark.Run("UnsafeEx SET", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        UnsafeEx.GetRef<int>(arr, (byte*)(ptr + 2 * Helper<int>.ElemSize)) = i;
                        sumUnsafeX++;
                    }
                }
            }

            Benchmark.Dump();
        }

        [Test]
        public void CouldGetSetViaMethodPointer()
        {
            var getterPtr = UnsafeEx.GetMethodPointerForType(typeof(int));
            var setterPtr = UnsafeEx.SetMethodPointerForType(typeof(int));

            Console.WriteLine((long)getterPtr);

            var arr = new int[] { 1, 2, 3 };

            var offset = UnsafeEx.ElemOffset(arr);
            Console.WriteLine(offset);

            var snd = UnsafeEx.Get<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(2, snd);

            UnsafeEx.SetIndirect(arr, (byte*)(offset + 4), (object)42, setterPtr);

            snd = (int)UnsafeEx.GetIndirect(arr, (byte*)(offset + 4), getterPtr);
            Assert.AreEqual(42, snd);
        }

        [Test, Explicit("long running")]
        public void CouldGetViaMethodPointerBenchmark()
        {
            var getterPtr = UnsafeEx.GetMethodPointerForType(typeof(int));
            var setterPtr = UnsafeEx.SetMethodPointerForType(typeof(int));

            var arr = new int[] { 1, 2, 3 };

            var offset = UnsafeEx.ElemOffset(arr);
            Console.WriteLine(offset);

            var snd = UnsafeEx.Get<int>(arr, (byte*)(offset + 4));
            Assert.AreEqual(2, snd);

            UnsafeEx.Set(arr, (byte*)(offset + 4), 42);

            snd = (int)UnsafeEx.GetIndirect(arr, (byte*)(offset + 4), getterPtr);
            Assert.AreEqual(42, snd);

            var count = 100_000_000;
            var sum = 0;
            for (int r = 0; r < 10; r++)
            {
                using (Benchmark.Run("GetIndirect", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        sum += (int)UnsafeEx.GetIndirect(arr, (byte*)(offset + 4), getterPtr);
                    }
                }

                using (Benchmark.Run("SetIndirect", count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        UnsafeEx.SetIndirect(arr, (byte*)(offset + 4), 42, setterPtr);
                    }
                }
            }

            Console.WriteLine(sum);
            Benchmark.Dump();
        }
    }
}