using System;
using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

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
            var baseJob = Job.Default
                .WithWarmupCount(1) // 1 warmup is enough for our purpose
                .WithIterationTime(TimeInterval.FromMilliseconds(250.0)) // the default is 0.5s per iteration
                .WithIterationCount(20)
                .WithMaxRelativeError(0.01);

            var jobBefore = baseJob.WithId("Core31").WithRuntime(CoreRuntime.Core31);

            var jobAfter =  baseJob.WithId("Core50").WithRuntime(CoreRuntime.Core50);

            var config = DefaultConfig.Instance.AddJob(jobBefore).AddJob(jobAfter).KeepBenchmarkFiles();

            BenchmarkRunner.Run<CpuIdBench>(config);

            // Trace.Listeners.Add(new ConsoleListener());

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

            // Console.WriteLine("Finished, press enter to exit...");
            //Console.ReadLine();
        }


    }
}
