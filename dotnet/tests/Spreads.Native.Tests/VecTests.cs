// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using NUnit.Framework;
using System;

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public class VecTests
    {
        [Test]
        public void CouldUseVec()
        {
            var arr = new[] { 1, 2, 3 };
            var vecT = new Vec<int>(arr);
            var vec = new Vec(arr);

            Assert.AreEqual(2, vecT[1]);

            vecT[1] = 42;
            vec[2] = (byte)123; // dynamic cast inside

            Assert.AreEqual(3, vecT.Length);
            Assert.AreEqual(3, vec.Length);

            Assert.AreEqual(42, vecT[1]);
            Assert.AreEqual(123, vecT[2]);

            Assert.AreEqual(42, vec[1]);
            Assert.AreEqual(123, vec[2]);

            Assert.Throws<IndexOutOfRangeException>(() => { vecT[3] = 42; });
        }
    }
}