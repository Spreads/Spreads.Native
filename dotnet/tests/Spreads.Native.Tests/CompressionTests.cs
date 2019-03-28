// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable PossibleNullReferenceException

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public unsafe class CompressionTests
    {
        public const long Iterations = 1_0;
        public const int ItemCount = 1_000;

        [Test]
        public void CouldShuffleUnshuffle()
        {
            byte itemSize = 16;
            var srcLen = ItemCount * itemSize;

            var originalPtr = (byte*)Marshal.AllocHGlobal(srcLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(srcLen);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(srcLen);

            for (int i = 0; i < ItemCount; i++)
            {
                for (int j = 0; j < itemSize; j++)
                {
                    originalPtr[i * itemSize + j] = (byte)(j + 1);
                }
            }

            var iterations = Iterations;

            var rounds = 3;
            for (int r = 0; r < rounds; r++)
            {
                using (Benchmark.Run("Shuffle", iterations * srcLen))
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Compression.shuffle((IntPtr)itemSize, (IntPtr)srcLen, originalPtr, compressedPtr);
                    }
                }
                using (Benchmark.Run("Unshuffle", iterations * srcLen))
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Compression.unshuffle((IntPtr)itemSize, (IntPtr)srcLen, compressedPtr, decompressedPtr);
                    }
                }
            }
            Benchmark.Dump();

            Assert.IsTrue(new Span<byte>(originalPtr, srcLen).SequenceEqual(new Span<byte>(decompressedPtr, srcLen)));
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TestValue
        {
            public int Num { get; set; }
            public int Num1 { get; set; }

            //public double Dbl { get; set; }
            //public double Dbl1 { get; set; }
        }

        [Test]
        public void Lz4Benchmark()
        {
            const string name = "LZ4";
            var count = ItemCount;

            var bufferLen = count * Unsafe.SizeOf<TestValue>();
            var originalPtr = (byte*)Marshal.AllocHGlobal(bufferLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen * 2);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen);

            for (int i = 0; i < count; i++)
            {
                ((TestValue*)originalPtr)[i] = new TestValue()
                {
                    //Dbl = (double)i + 1 / (double)(i + 1),
                    //Dbl1 = (double)i + 1 / (double)(i + 1),
                    Num = i,
                    Num1 = i,
                };
            }

            for (int level = 1; level < 10; level++)
            {
                var compressedLen = Compression.compress_lz4(originalPtr, (IntPtr)bufferLen, compressedPtr,
                    (IntPtr)(bufferLen * 2), level);

                var decompressedLen = Compression.decompress_lz4(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);

                Console.WriteLine($"{name} Level: {level}, ratio: {1.0 * decompressedLen / compressedLen}");

                Assert.AreEqual(bufferLen, decompressedLen);
                Assert.IsTrue(new Span<byte>(originalPtr, bufferLen).SequenceEqual(new Span<byte>(decompressedPtr, bufferLen)));
            }

            Console.WriteLine("-------------------------------");

            var rounds = 3;
            var iterations = Iterations / 10;
            for (int r = 0; r < rounds; r++)
            {
                for (int level = 1; level < 10; level++)
                {
                    int compressedLen = 0;
                    using (Benchmark.Run($"{name} W{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            compressedLen = Compression.compress_lz4(originalPtr, (IntPtr)bufferLen, compressedPtr,
                                (IntPtr)bufferLen, level);
                        }
                    }

                    using (Benchmark.Run($"{name} R{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            Compression.decompress_lz4(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);
                        }
                    }
                }
            }

            Benchmark.Dump();
        }

        [Test]
        public void ZstdBenchmark()
        {
            const string name = "Zstd";
            var count = ItemCount;

            var bufferLen = count * Unsafe.SizeOf<TestValue>();
            var originalPtr = (byte*)Marshal.AllocHGlobal(bufferLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen * 2);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen);

            for (int i = 0; i < count; i++)
            {
                ((TestValue*)originalPtr)[i] = new TestValue()
                {
                    //Dbl = (double)i + 1 / (double)(i + 1),
                    //Dbl1 = (double)i + 1 / (double)(i + 1),
                    Num = i,
                    Num1 = i,
                };
            }

            for (int level = 1; level < 10; level++)
            {
                var compressedLen = Compression.compress_zstd(originalPtr, (IntPtr)bufferLen, compressedPtr,
                    (IntPtr)(bufferLen * 2), level);

                var decompressedLen = Compression.decompress_zstd(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);

                Console.WriteLine($"{name} Level: {level}, ratio: {1.0 * decompressedLen / compressedLen}");

                Assert.AreEqual(bufferLen, decompressedLen);
                Assert.IsTrue(new Span<byte>(originalPtr, bufferLen).SequenceEqual(new Span<byte>(decompressedPtr, bufferLen)));
            }

            Console.WriteLine("-------------------------------");

            var rounds = 3;
            var iterations = Iterations / 100;
            for (int r = 0; r < rounds; r++)
            {
                for (int level = 1; level < 10; level++)
                {
                    int compressedLen = 0;
                    using (Benchmark.Run($"{name} W{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            compressedLen = Compression.compress_zstd(originalPtr, (IntPtr)bufferLen, compressedPtr,
                                (IntPtr)bufferLen, level);
                        }
                    }

                    using (Benchmark.Run($"{name} R{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            Compression.decompress_zstd(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);
                        }
                    }
                }
            }

            Benchmark.Dump();
        }

        [Test]
        public void DeflateBenchmark()
        {
            const string name = "Deflate";
            var count = ItemCount;

            var bufferLen = count * Unsafe.SizeOf<TestValue>();
            var originalPtr = (byte*)Marshal.AllocHGlobal(bufferLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen * 2);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen);

            for (int i = 0; i < count; i++)
            {
                ((TestValue*)originalPtr)[i] = new TestValue()
                {
                    //Dbl = (double)i + 1 / (double)(i + 1),
                    //Dbl1 = (double)i + 1 / (double)(i + 1),
                    Num = i,
                    Num1 = i,
                };
            }

            for (int level = 1; level < 10; level++)
            {
                var compressedLen = Compression.compress_deflate(originalPtr, (IntPtr)bufferLen, compressedPtr,
                    (IntPtr)(bufferLen * 2), level);

                var decompressedLen = Compression.decompress_deflate(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);

                Console.WriteLine($"{name} Level: {level}, ratio: {1.0 * decompressedLen / compressedLen}");

                Assert.AreEqual(bufferLen, decompressedLen);
                Assert.IsTrue(new Span<byte>(originalPtr, bufferLen).SequenceEqual(new Span<byte>(decompressedPtr, bufferLen)));
            }

            Console.WriteLine("-------------------------------");

            var rounds = 3;
            var iterations = Iterations / 100;
            for (int r = 0; r < rounds; r++)
            {
                for (int level = 1; level < 10; level++)
                {
                    int compressedLen = 0;
                    using (Benchmark.Run($"{name} W{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            compressedLen = Compression.compress_deflate(originalPtr, (IntPtr)bufferLen, compressedPtr,
                                (IntPtr)bufferLen, level);
                        }
                    }

                    using (Benchmark.Run($"{name} R{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            Compression.decompress_deflate(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);
                        }
                    }
                }
            }

            Benchmark.Dump();
        }

        [Test]
        public void GzipBenchmark()
        {
            const string name = "Gzip";
            var count = ItemCount;

            var bufferLen = count * Unsafe.SizeOf<TestValue>();
            var originalPtr = (byte*)Marshal.AllocHGlobal(bufferLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen * 2);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen);

            for (int i = 0; i < count; i++)
            {
                ((TestValue*)originalPtr)[i] = new TestValue()
                {
                    //Dbl = (double)i + 1 / (double)(i + 1),
                    //Dbl1 = (double)i + 1 / (double)(i + 1),
                    Num = i,
                    Num1 = i,
                };
            }

            for (int level = 1; level < 10; level++)
            {
                var compressedLen = Compression.compress_gzip(originalPtr, (IntPtr)bufferLen, compressedPtr,
                    (IntPtr)(bufferLen * 2), level);

                var decompressedLen = Compression.decompress_gzip(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);

                Console.WriteLine($"{name} Level: {level}, ratio: {1.0 * decompressedLen / compressedLen}");

                Assert.AreEqual(bufferLen, decompressedLen);
                Assert.IsTrue(new Span<byte>(originalPtr, bufferLen).SequenceEqual(new Span<byte>(decompressedPtr, bufferLen)));
            }

            Console.WriteLine("-------------------------------");

            var rounds = 3;
            var iterations = Iterations / 100;
            for (int r = 0; r < rounds; r++)
            {
                for (int level = 1; level < 10; level++)
                {
                    int compressedLen = 0;
                    using (Benchmark.Run($"{name} W{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            compressedLen = Compression.compress_gzip(originalPtr, (IntPtr)bufferLen, compressedPtr,
                                (IntPtr)bufferLen, level);
                        }
                    }

                    using (Benchmark.Run($"{name} R{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            Compression.decompress_gzip(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);
                        }
                    }
                }
            }

            Benchmark.Dump();
        }

        [Test]
        public void ZlibBenchmark()
        {
            const string name = "Zlib";
            var count = ItemCount;

            var bufferLen = count * Unsafe.SizeOf<TestValue>();
            var originalPtr = (byte*)Marshal.AllocHGlobal(bufferLen);
            var compressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen * 2);
            var decompressedPtr = (byte*)Marshal.AllocHGlobal(bufferLen);

            for (int i = 0; i < count; i++)
            {
                ((TestValue*)originalPtr)[i] = new TestValue()
                {
                    //Dbl = (double)i + 1 / (double)(i + 1),
                    //Dbl1 = (double)i + 1 / (double)(i + 1),
                    Num = i,
                    Num1 = i,
                };
            }

            for (int level = 1; level < 10; level++)
            {
                var compressedLen = Compression.compress_zlib(originalPtr, (IntPtr)bufferLen, compressedPtr,
                    (IntPtr)(bufferLen * 2), level);

                var decompressedLen = Compression.decompress_zlib(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);

                Console.WriteLine($"{name} Level: {level}, ratio: {1.0 * decompressedLen / compressedLen}");

                Assert.AreEqual(bufferLen, decompressedLen);
                Assert.IsTrue(new Span<byte>(originalPtr, bufferLen).SequenceEqual(new Span<byte>(decompressedPtr, bufferLen)));
            }

            Console.WriteLine("-------------------------------");

            var rounds = 3;
            var iterations = Iterations / 100;
            for (int r = 0; r < rounds; r++)
            {
                for (int level = 1; level < 10; level++)
                {
                    int compressedLen = 0;
                    using (Benchmark.Run($"{name} W{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            compressedLen = Compression.compress_zlib(originalPtr, (IntPtr)bufferLen, compressedPtr,
                                (IntPtr)bufferLen, level);
                        }
                    }

                    using (Benchmark.Run($"{name} R{level}", bufferLen * iterations, true))
                    {
                        for (int i = 0; i < iterations; i++)
                        {
                            Compression.decompress_zlib(compressedPtr, (IntPtr)compressedLen, decompressedPtr, (IntPtr)bufferLen);
                        }
                    }
                }
            }

            Benchmark.Dump();
        }
    }
}