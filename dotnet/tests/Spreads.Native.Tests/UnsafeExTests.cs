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
        public static object NullObj = null;

        public static readonly int ElemOffset = (int)UnsafeEx.ArrayOffsetAdjustment<int>();
        public static readonly int ElemSize = Unsafe.SizeOf<int>();

        public static class Helper<T>
        {
            public static readonly int ElemOffset = (int)UnsafeEx.ArrayOffsetAdjustment<T>();
            public static readonly int ElemSize = Unsafe.SizeOf<T>();
        }

        [Test]
        public void UnsafeExWorks()
        {
            var arr = new int[] { 1, 2, 3 };
            object obj = arr;
            var offset = (int)UnsafeEx.ArrayOffsetAdjustmentOfType(arr.GetType());

            //var offsetO = (int) UnsafeEx.ElemOffset(new int[1]);
            //var offsetA = UnsafeEx.ArrayOffsetAdjustment<int>();
            //var fpa = offsetO - offsetA;
            var snd = UnsafeEx.Get<int>(ref obj, (IntPtr)(offset), 1);
            Assert.AreEqual(2, snd);

            UnsafeEx.GetRef<int>(arr, (IntPtr)(offset), 1) = 42;

            snd = UnsafeEx.GetRef<int>(arr, (IntPtr)(offset), 1);
            Assert.AreEqual(42, snd);
        }

        //[Test]
        //public void UnsafeExWorksWithStrings()
        //{
        //    var arr = new string[] { "1", "2", "3" };

        //    var offset = UnsafeEx.ArrayOffsetAdjustmentOfType(arr.GetType());
        //    Console.WriteLine("Offset: " + offset);
        //    Console.WriteLine("Element size: " + Helper<string>.ElemSize);

        //    var snd = UnsafeEx.DangerousGetAtIndex<string>(arr, (IntPtr)(offset), 1);
        //    Assert.AreEqual("2", snd);

        //    UnsafeEx.GetRef<string>(arr, (IntPtr)(offset), 1) = "42";

        //    snd = UnsafeEx.GetRef<string>(arr, (IntPtr)(offset + Helper<string>.ElemSize), 0);
        //    Assert.AreEqual("42", snd);
        //}

        [Test]
        public void UnsafeExWorksViaPinnedPtr()
        {
            var arr = new int[] { 1, 2, 3 };
            
            var handle = ((Memory<int>)arr).Pin();
            var ptr = (IntPtr)handle.Pointer;

            var offset = UnsafeEx.ArrayOffsetAdjustmentOfType(arr.GetType());
            Console.WriteLine(offset);

            var snd = UnsafeEx.Get<int>(ref NullObj, (IntPtr)(ptr), 1);
            Assert.AreEqual(2, snd);

            UnsafeEx.GetRef<int>(arr, (IntPtr)(offset - 8), 1) = 42;

            snd = UnsafeEx.GetRef<int>(arr, (IntPtr)(offset - 8), 1);
            Assert.AreEqual(42, snd);

            snd = UnsafeEx.Get<int>(ref NullObj, (IntPtr)(ptr), 1);
            Assert.AreEqual(42, snd);
        }

        //[Test]
        //public void UnsafeExWorksWithNull()
        //{
        //    var ptr = (IntPtr)Marshal.AllocHGlobal(64);

        //    int[] arr = null;

        //    *(((int*)ptr) + 1) = 2;

        //    var snd = UnsafeEx.DangerousGetAtIndex<int>(arr, (IntPtr)(ptr), 1);
        //    Assert.AreEqual(2, snd);

        //    UnsafeEx.GetRef<int>(arr, (IntPtr)(ptr), 1) = 42;

        //    snd = UnsafeEx.GetRef<int>(arr, (IntPtr)(ptr), 1);
        //    Assert.AreEqual(42, snd);
        //}

        //[Test]
        //public void UnsafeExWorksWithNullWrongButCompatibleType()
        //{
        //    var ptr = (IntPtr)Marshal.AllocHGlobal(64);

        //    int[] arr = null;

        //    *(((int*)ptr) + 1) = 2;

        //    var snd = UnsafeEx.DangerousGetAtIndex<int>(arr, (IntPtr)(ptr + 4), 0);
        //    Assert.AreEqual(2, snd);

        //    UnsafeEx.GetRef<int>(arr, (IntPtr)(ptr), 1) = 42;

        //    snd = UnsafeEx.GetRef<int>(arr, (IntPtr)(ptr), 1);
        //    Assert.AreEqual(42, snd);
        //}

        //[Test, Explicit("Long running")]
        //public void GetSetBench()
        //{
        //    var arr = new int[] { 1, 2, 3 };

        //    var count = 100_000_000;
        //    var rounds = 10;

        //    for (int r = 0; r < rounds; r++)
        //    {
        //        long sumArr = 0;
        //        using (Benchmark.Run("Arr GET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                sumArr += arr[2];
        //            }
        //        }

        //        long sumUnsafe = 0;
        //        using (Benchmark.Run("UnsafeEx GET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                sumUnsafe += UnsafeEx.GetRef<int>(arr, (IntPtr)(Helper<int>.ElemOffset), 2);
        //            }
        //        }

        //        long sumArrX = 0;
        //        using (Benchmark.Run("Arr SET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                arr[2] = i;
        //                sumArrX++;
        //            }
        //        }

        //        long sumUnsafeX = 0;
        //        using (Benchmark.Run("UnsafeEx SET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                UnsafeEx.GetRef<int>(arr, (IntPtr)(Helper<int>.ElemOffset), 2) = i;
        //                sumUnsafeX++;
        //            }
        //        }

        //        Assert.AreEqual(sumArr, sumUnsafe);
        //        Assert.AreEqual(sumArrX, sumUnsafeX);

        //        UnsafeEx.GetRef<int>(arr, (IntPtr)(VecTypeHelper<int>.RuntimeVecInfo.ArrayOffsetAdjustment), 2) = arr[2]++;
        //    }

        //    Benchmark.Dump();
        //}

        //[Test, Explicit("Long running")]
        //public void GetSetBenchPinned()
        //{
        //    var arr = new int[] { 1, 2, 3 };
        //    var handle = ((Memory<int>)arr).Pin();
        //    var ptr = (IntPtr)handle.Pointer;

        //    var count = 100_000_000;
        //    var rounds = 10;

        //    for (int r = 0; r < rounds; r++)
        //    {
        //        long sumArr = 0;
        //        using (Benchmark.Run("Arr GET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                sumArr += arr[2];
        //            }
        //        }

        //        long sumUnsafe = 0;
        //        using (Benchmark.Run("UnsafeEx GET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                sumUnsafe += UnsafeEx.GetRef<int>(null, (IntPtr)(ptr), 2);
        //            }
        //        }

        //        long sumArrX = 0;
        //        using (Benchmark.Run("Arr SET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                arr[2] = i;
        //                sumArrX++;
        //            }
        //        }

        //        long sumUnsafeX = 0;
        //        using (Benchmark.Run("UnsafeEx SET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                UnsafeEx.GetRef<int>(null, (IntPtr)(ptr), 2) = i;
        //                sumUnsafeX++;
        //            }
        //        }

        //        Assert.AreEqual(sumArr, sumUnsafe);
        //        Assert.AreEqual(sumArrX, sumUnsafeX);

        //        UnsafeEx.GetRef<int>(arr, (IntPtr)(VecTypeHelper<int>.RuntimeVecInfo.ArrayOffsetAdjustment), 2) = arr[2]++;
        //    }

        //    Benchmark.Dump();
        //}

        //[Test, Explicit("Long running")]
        //public void GetSetBenchWithNull()
        //{
        //    int[] arr = null;

        //    var arr2 = new int[] { 1, 2, 3 };

        //    var ptr = (IntPtr)Marshal.AllocHGlobal(64);

        //    var count = 100_000_000;
        //    var rounds = 10;

        //    for (int r = 0; r < rounds; r++)
        //    {
        //        long sumUnsafe = 0;
        //        using (Benchmark.Run("UnsafeEx GET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                sumUnsafe += UnsafeEx.DangerousGetAtIndex<int>(arr, (IntPtr)(ptr + 2 * Helper<int>.ElemSize), 0);
        //            }
        //        }

        //        long sumUnsafeX = 0;
        //        using (Benchmark.Run("UnsafeEx SET", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                UnsafeEx.GetRef<int>(arr, (IntPtr)(ptr), 2) = i;
        //                sumUnsafeX++;
        //            }
        //        }
        //    }

        //    Benchmark.Dump();
        //}

        //[Test]
        //public void CouldGetSetViaMethodPointer()
        //{
        //    var getterPtr = UnsafeEx.GetMethodPointerForType(typeof(int));
        //    var setterPtr = UnsafeEx.SetMethodPointerForType(typeof(int));

        //    Console.WriteLine((long)getterPtr);

        //    var arr = new int[] { 1, 2, 3 };

        //    var offset = UnsafeEx.ArrayOffsetAdjustmentOfType(arr.GetType());
        //    Console.WriteLine(offset);

        //    var snd = UnsafeEx.DangerousGetAtIndex<int>(arr, (IntPtr)(offset + 4), 0);
        //    Assert.AreEqual(2, snd);

        //    UnsafeEx.SetIndirect(arr, (IntPtr)(offset), 1, (object)42, setterPtr);

        //    snd = (int)UnsafeEx.GetIndirect(arr, (IntPtr)(offset), 1, getterPtr);
        //    Assert.AreEqual(42, snd);
        //}

        //internal delegate void SetXDelegate<T>(object obj, IntPtr offset, object val);

        //internal delegate object GetXDelegate<T>(object obj, IntPtr offset);

        //internal abstract class SetDel
        //{
        //    public abstract void Call(object obj, IntPtr offset, object val);
        //}

        //internal sealed class SetDel<T> : SetDel
        //{
        //    private readonly SetXDelegate<T> _del;

        //    public SetDel(SetXDelegate<T> del)
        //    {
        //        _del = del;
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public override void Call(object obj, IntPtr offset, object val)
        //    {
        //        _del(obj, offset, val);
        //    }
        //}

        //[Test, Explicit("long running")]
        //public void CouldGetViaMethodPointerBenchmark()
        //{
        //    var getterPtr = UnsafeEx.GetMethodPointerForType(typeof(int));
        //    var setterPtr = UnsafeEx.SetMethodPointerForType(typeof(int));

        //    MethodInfo setMethod = typeof(UnsafeEx).GetMethod("SetX", BindingFlags.Static | BindingFlags.NonPublic);
        //    MethodInfo setGenericMethod = setMethod.MakeGenericMethod(typeof(int));
        //    var setDelegate = (SetXDelegate<int>)setGenericMethod.CreateDelegate(typeof(SetXDelegate<int>));
        //    SetDel setDel = new SetDel<int>(setDelegate);

        //    MethodInfo getMethod = typeof(UnsafeEx).GetMethod("GetX", BindingFlags.Static | BindingFlags.NonPublic);
        //    MethodInfo getGenericMethod = getMethod.MakeGenericMethod(typeof(int));
        //    var getDelegate = (GetXDelegate<int>)getGenericMethod.CreateDelegate(typeof(GetXDelegate<int>));

        //    var arr = new int[] { 1, 2, 3 };
        //    Array uArr = arr;
        //    var offset = UnsafeEx.ArrayOffsetAdjustmentOfType(arr.GetType());
        //    Console.WriteLine(offset);

        //    var snd = UnsafeEx.DangerousGetAtIndex<int>(arr, (IntPtr)(offset + 4), 0);
        //    Assert.AreEqual(2, snd);

        //    UnsafeEx.GetRef<int>(arr, (IntPtr)(offset), 1) = 42;

        //    snd = (int)UnsafeEx.GetIndirect(arr, (IntPtr)(offset), 1, getterPtr);
        //    Assert.AreEqual(42, snd);

        //    var count = 10_000_000;
        //    var sum = 0;
        //    for (int r = 0; r < 50; r++)
        //    {
        //        using (Benchmark.Run("GetArray", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                unchecked
        //                {
        //                    sum += (int)uArr.GetValue(1);
        //                }
        //            }
        //        }

        //        using (Benchmark.Run("GetIndirect", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                unchecked
        //                {
        //                    sum += (int)UnsafeEx.GetIndirect(arr, (IntPtr)(offset), 1, getterPtr);
        //                }
        //            }
        //        }

        //        using (Benchmark.Run("GetDelegate", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                unchecked
        //                {
        //                    sum += (int)getDelegate.Invoke(arr, (IntPtr)(IntPtr)(offset + 4));
        //                }
        //            }
        //        }

        //        using (Benchmark.Run("SetArray", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                uArr.SetValue(42, 1);
        //            }
        //        }

        //        using (Benchmark.Run("SetIndirect", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                UnsafeEx.SetIndirect(arr, (IntPtr)(offset), 1, 42, setterPtr);
        //            }
        //        }

        //        using (Benchmark.Run("SetDelegate", count))
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                setDel.Call(arr, (IntPtr)(IntPtr)(offset + 4), 42);
        //            }
        //        }
        //    }

        //    Console.WriteLine(sum);
        //    Benchmark.Dump();
        //}
    }
}