using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using System;

namespace Spreads.Native.Run
{
    [Config(typeof(MultipleRuntimesConfig))]
    [MarkdownExporterAttribute.Default]
    [MarkdownExporterAttribute.GitHub]
    public class Benchmark
    {
        public const int Loops = 100;
        public const int Count = 10_000;

        private int[] _arr;
        private Vec<int> _vecT;
        private Vec _vec;
        private Memory<int> _mem;

        public volatile int _count = Count;

        public Benchmark()
        {
            _arr = new int[Count];
            _vecT = new Vec<int>(_arr);
            _vec = new Vec(_arr);
            _mem = (Memory<int>)_arr;
        }

        [Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "Vec<T>.GetUnchecked(i)")]
        public int VecTIndexer_Get()
        {
            int sum = 0;
            for (int _ = 0; _ < Loops; _++)
            {
                for (int j = 1; j < _count; j++)
                {
                    if (j >= 42)
                    {
                        sum += _vecT.GetUnchecked(j - 1);
                    }
                    else
                    {
                        sum += _vecT.GetUnchecked(j);
                    }
                }
            }

            return sum;
        }

        //[Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "Vec.GetUnchecked<T>(i)")]
        //public int VecGetTIndexer_Get()
        //{
        //    int sum = 0;

        //    for (int _ = 0; _ < Loops; _++)
        //    {
        //        for (int j = 1; j < _count; j++)
        //        {
        //            if (j >= 42)
        //            {
        //                sum += _vec.GetUnchecked<int>(j - 1);
        //            }
        //            else
        //            {
        //                sum += _vec.GetUnchecked<int>(j);
        //            }
        //        }
        //    }

        //    return sum;
        //}

        //[Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "(T)Vec.GetUnchecked(i)")]
        //public int VecGetIndexer_Get()
        //{
        //    int sum = 0;

        //    for (int _ = 0; _ < Loops; _++)
        //    {
        //        for (int j = 1; j < _count; j++)
        //        {
        //            if (j >= 42)
        //            {
        //                sum += (int)_vec.GetUnchecked(j - 1);
        //            }
        //            else
        //            {
        //                sum += (int)_vec.GetUnchecked(j);
        //            }
        //        }
        //    }

        //    return sum;
        //}

        [Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "T[i]")]
        public int ArrayIndexer_Get()
        {
            int sum = 0;
            for (int _ = 0; _ < Loops; _++)
            {
                for (int j = 1; j < _count; j++)
                {
                    if (j >= 42)
                    {
                        sum += _arr[j - 1];
                    }
                    else
                    {
                        sum += _arr[j];
                    }
                }
            }

            return sum;
        }

        //[Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "T[i] NO BC")]
        //public int ArrayIndexerNoBC_Get()
        //{
        //    int sum = 0;
        //    for (int _ = 0; _ < Loops; _++)
        //    {
        //        for (int j = 0; j < _arr.Length; j++)
        //        {
        //            if (j >= 2)
        //            {
        //                sum += _arr[j];
        //            }
        //        }
        //    }

        //    return sum;
        //}

        [Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "Span<T>[i]")]
        public int SpanIndexer_Get()
        {
            int sum = 0;
            Span<int> local = _arr; // implicit cast to Span, we can't have Span as a field!
            for (int _ = 0; _ < Loops; _++)
            {
                for (int j = 1; j < _count; j++)
                {
                    if (j >= 42)
                    {
                        sum += local[j - 1];
                    }
                    else
                    {
                        sum += local[j];
                    }
                }
            }

            return sum;
        }

        //[Benchmark(OperationsPerInvoke = Loops * (Count - 2), Description = "Memory<T>.Span[i]")]
        //public int MemoryIndexer_Get()
        //{
        //    int sum = 0;

        //    for (int _ = 0; _ < Loops; _++)
        //    {
        //        for (int j = 1; j < _count; j++)
        //        {
        //            if (j >= 42)
        //            {
        //                sum += _mem.Span[j - 1];
        //            }
        //            else
        //            {
        //                sum += _mem.Span[j];
        //            }
        //        }
        //    }

        //    return sum;
        //}
    }

    public class MultipleRuntimesConfig : ManualConfig
    {
        public MultipleRuntimesConfig()
        {
            Add(StatisticColumn.Median);
            Add(StatisticColumn.OperationsPerSecond);
            Add(MarkdownExporter.Default);
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);

            Add(Job.Default
                .With(CsProjClassicNetToolchain.Net461)
                .WithId(".NET 4.6.1"));

            Add(Job.Default
                .With(CsProjCoreToolchain.NetCoreApp21)
                .WithId(".NET Core 2.1 LTS"));

            Add(Job.Default
                .With(CsProjCoreToolchain.NetCoreApp30)
                .WithId(".NET Core 3.0.1-pv"));
        }
    }
}
