using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Spreads.Native.Tests
{
    [Category("CI")]
    [TestFixture]
    public class ProcessorIdCacheTests
    {
        [Test]
        public void CouldGetCpuNumberFromCache()
        {
            var nativeCpuId = ProcessorIdCache.get_cpu_number();
            Console.WriteLine($"native: {nativeCpuId}");

            var cpuId = ProcessorIdCache.GetCurrentProcessorId();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.IsTrue(nativeCpuId >= 0);
            }

            Assert.IsTrue(cpuId >= 0);
            Console.WriteLine($"cached: {cpuId}");
        }
    }
}