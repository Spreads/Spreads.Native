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
        public static object NullObj = null;

        public static readonly int ElemSize = Unsafe.SizeOf<int>();

        [Test]
        public void CeqCgtCltWork()
        {
            // int32
            Assert.AreEqual(1, UnsafeEx.Ceq(1, 1));
            Assert.AreEqual(0, UnsafeEx.Ceq(1, 2));

            Assert.AreEqual(1, UnsafeEx.Cgt(2, 1));
            Assert.AreEqual(0, UnsafeEx.Cgt(1, 1));
            Assert.AreEqual(0, UnsafeEx.Cgt(0, 1));

            Assert.AreEqual(1, UnsafeEx.Clt(1, 2));
            Assert.AreEqual(0, UnsafeEx.Clt(1, 1));
            Assert.AreEqual(0, UnsafeEx.Clt(1, 0));

            // int64
            Assert.AreEqual(1, UnsafeEx.Ceq(1L, 1L));
            Assert.AreEqual(0, UnsafeEx.Ceq(1L, 2L));

            Assert.AreEqual(1, UnsafeEx.Cgt(2L, 1L));
            Assert.AreEqual(0, UnsafeEx.Cgt(1L, 1L));
            Assert.AreEqual(0, UnsafeEx.Cgt(0L, 1L));

            Assert.AreEqual(1, UnsafeEx.Clt(1L, 2L));
            Assert.AreEqual(0, UnsafeEx.Clt(1L, 1L));
            Assert.AreEqual(0, UnsafeEx.Clt(1L, 0L));

            // explicit int32 -> int64
            Assert.AreEqual(1, UnsafeEx.Ceq(1, 1L));
            Assert.AreEqual(0, UnsafeEx.Ceq(1, 2L));

            Assert.AreEqual(1, UnsafeEx.Cgt(2, 1L));
            Assert.AreEqual(0, UnsafeEx.Cgt(1, 1L));
            Assert.AreEqual(0, UnsafeEx.Cgt(0, 1L));

            Assert.AreEqual(1, UnsafeEx.Clt(1, 2L));
            Assert.AreEqual(0, UnsafeEx.Clt(1, 1L));
            Assert.AreEqual(0, UnsafeEx.Clt(1, 0L));

            // IntPtr
            Assert.AreEqual(1, UnsafeEx.Ceq((IntPtr) 1, (IntPtr) 1));
            Assert.AreEqual(0, UnsafeEx.Ceq((IntPtr) 1, (IntPtr) 2));

            Assert.AreEqual(1, UnsafeEx.Cgt((IntPtr) 2, (IntPtr) 1));
            Assert.AreEqual(0, UnsafeEx.Cgt((IntPtr) 1, (IntPtr) 1));
            Assert.AreEqual(0, UnsafeEx.Cgt((IntPtr) 0, (IntPtr) 1));

            Assert.AreEqual(1, UnsafeEx.Clt((IntPtr) 1, (IntPtr) 2));
            Assert.AreEqual(0, UnsafeEx.Clt((IntPtr) 1, (IntPtr) 1));
            Assert.AreEqual(0, UnsafeEx.Clt((IntPtr) 1, (IntPtr) 0));

            // Float
            Assert.AreEqual(1, UnsafeEx.Ceq(1.23, 1.23));
            Assert.AreEqual(0, UnsafeEx.Ceq(1.23, 1.24));

            Assert.AreEqual(1, UnsafeEx.Ceq(1.23f, 1.23f));
            Assert.AreEqual(0, UnsafeEx.Ceq(1.23f, 1.24f));

            Assert.AreEqual(1, UnsafeEx.BoolAsInt(true));
            Assert.AreEqual(0, UnsafeEx.BoolAsInt(false));
        }


    }
}
