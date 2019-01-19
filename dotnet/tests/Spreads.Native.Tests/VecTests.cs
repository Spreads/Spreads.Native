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

        [Test, Explicit("long running")]
        public void ForEachBench()
        {
            var count = 50_000_000;
            var arr = new int[count];
            var vecT = new Vec<int>(arr);
            var vec = new Vec(arr);
            var mem = (Memory<int>)arr;

            for (int i = 0; i < count; i++)
            {
                vecT[i] = i;
                //if ((int)vec[i] != vecT[i])
                //{
                //    throw new Exception("(int)vec[i] != vecT[i]");
                //}
            }

            long sum = 0;
            var rounds = 20;
            var mult = 10;

            for (int r = 0; r < rounds; r++)
            {
                //using (Benchmark.Run("Array", count * mult))
                //{
                //    var z = count - 1;
                //    for (int m = 0; m < mult; m++)
                //    {
                //        for (int j = 1; j < z; j++)
                //        {
                //            sum += arr[j - 1];
                //        }
                //    }
                //}

                using (Benchmark.Run("VecT", count * mult))
                {
                    for (int m = 0; m < mult; m++)
                    {
                        var z = count - 1;
                        for (int j = 1; j < z; j++)
                        {
                            sum += vecT.GetUnchecked(j - 1);
                        }
                    }
                }

                //using (Benchmark.Run("Span", count * mult))
                //{
                //    for (int m = 0; m < mult; m++)
                //    {
                //        var z = count - 1;
                //        var sp = vecT.Span;
                //        for (int j = 1; j < z; j++)
                //        {
                //            sum += sp[j - 1];
                //        }
                //    }
                //}

                //using (Benchmark.Run("Vec.Get<T>", count * mult))
                //{
                //    for (int m = 0; m < mult; m++)
                //    {
                //        for (int j = 0; j < count; j++)
                //        {
                //            sum += vec.Get<int>(j);
                //        }
                //    }
                //}

                //using (Benchmark.Run("Vec", count * mult))
                //{
                //    for (int m = 0; m < mult; m++)
                //    {
                //        for (int j = 0; j < count; j++)
                //        {
                //            sum += (int)vec.GetUnchecked(j);
                //        }
                //    }
                //}

                //using (Benchmark.Run("Memory<T>.Span", count * mult))
                //{
                //    for (int m = 0; m < mult; m++)
                //    {
                //        for (int j = 0; j < count; j++)
                //        {
                //            sum += (int)mem.Span[j];
                //        }
                //    }
                //}
            }

            Benchmark.Dump();
            Console.WriteLine(sum);
        }
    }
}