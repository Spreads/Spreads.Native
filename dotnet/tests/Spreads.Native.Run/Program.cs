using System;
using System.Diagnostics;

namespace Spreads.Native.Run
{
    internal class ConsoleListener : TraceListener
    {
        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleListener());

            Console.WriteLine("CPU ID: " + Cpu.GetCurrentCoreId());
            // var summary = BenchmarkRunner.Run<Benchmark>();

            var test = new Tests.ProcessorIdCacheTests();
            test.CpuNumberPerformance();

            //var offset = UnsafeExTests.Helper<int>.ElemOffset;
            //var size = UnsafeExTests.Helper<int>.ElemSize;
            //Console.WriteLine("Offset: " + offset);
            //Console.WriteLine("Size: " + size);

            //var test = new CompressionTests();

            //Console.WriteLine("----------- Shuffle -----------");
            //test.CouldShuffleUnshuffle();

            //Console.WriteLine("----------- LZ4 -----------");
            //test.Lz4Benchmark();

            //Console.WriteLine("----------- ZSTD -----------");
            //test.ZstdBenchmark();

            //Console.WriteLine("----------- GZip -----------");
            //test.GzipBenchmark();

            //Console.WriteLine("----------- Deflate -----------");
            //test.DeflateBenchmark();

            //Console.WriteLine("\n\n\n-------------------------------");

            Console.WriteLine("Finished, press enter to exit...");
            //Console.ReadLine();
        }
    }
}
