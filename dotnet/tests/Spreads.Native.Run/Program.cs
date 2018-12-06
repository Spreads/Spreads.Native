using System;
using System.Diagnostics;
using Spreads.Native.Tests;

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

            var test = new CompressionTests();
            test.CouldShuffleUnshuffle();

            Console.WriteLine("Finished, press enter to exit...");
            Console.ReadLine();
        }

        
//        private static void CompressionBenchmark()
//        {
//            var test = new Tests.Blosc.BloscTests();
//            // test.CouldShuffleUnshuffle();

//            Console.WriteLine("----------- LZ4 -----------");
//            test.Lz4Benchmark();
//            Console.WriteLine("----------- ZSTD -----------");
//            test.ZstdBenchmark();
//            Console.WriteLine("----------- GZip -----------");
//            test.GZipBenchmark();
//#if NETCOREAPP2_1
//            Console.WriteLine("----------- Brotli -----------");
//            test.BrotliBenchmark();
//#endif
//            Console.WriteLine("----------- Copy -----------");
//            test.CopyViaCalliBenchmark();
//            //Console.WriteLine("----------- Deflate -----------");
//            //test.DeflateBenchmark();
//            Console.WriteLine("Finished, press enter to exit...");
//            Console.ReadLine();
//        }
    }
}