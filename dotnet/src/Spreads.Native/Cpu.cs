// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Spreads.Native
{
    [SuppressUnmanagedCodeSecurity]
    public static class Cpu
    {
        public const int MaxCores = 128;
        
        /// <summary>
        /// The number of cpu cores available for the current process, capped at <see cref="MaxCores"/>.
        /// </summary>
        public static readonly int CoreCount = Math.Min(Environment.ProcessorCount, MaxCores);
        
        [DllImport(UnsafeEx.NativeLibraryName, EntryPoint = "spreads_pal_get_cpu_number",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int spreads_pal_get_cpu_number();

        internal static int get_cpu_number()
        {
            try
            {
                return spreads_pal_get_cpu_number();
            }
            catch
            {
                return -1;
            }
        }

        // The upper bits of t_currentProcessorIdCache are the currentProcessorId. The lower bits of
        // the t_currentProcessorIdCache are counting down to get it periodically refreshed.
        [ThreadStatic]
        private static int _currentProcessorIdCache;

        private const int CacheShift = 16;

        private const int CacheCountDownMask = (1 << CacheShift) - 1;

        private const int RefreshRate = 1000;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RefreshCurrentCoreId()
        {
            int currentProcessorId = get_cpu_number();

            // On Unix, GetCurrentProcessorNumber() is implemented in terms of sched_getcpu, which
            // doesn't exist on all platforms.  On those it doesn't exist on, GetCurrentProcessorNumber()
            // returns -1.  As a fallback in that case and to spread the threads across the buckets
            // by default, we use the current managed thread ID as a proxy.
            if (currentProcessorId < 0)
                currentProcessorId = Environment.CurrentManagedThreadId;

            // Make CPU id a valid index from [0, CoreCount]
            currentProcessorId %= CoreCount;

            // Mask with int.MaxValue to ensure the execution Id is not negative
            _currentProcessorIdCache = ((currentProcessorId << CacheShift) & int.MaxValue) |
                                       RefreshRate;

            return currentProcessorId;
        }

        /// <summary>
        /// Returns a cached id of the current core. The value is always
        /// valid as an index of an array with a length of <see cref="CoreCount"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCurrentCoreId()
        {
            int currentProcessorIdCache = _currentProcessorIdCache--;
            if ((currentProcessorIdCache & CacheCountDownMask) == 0)
            {
                return RefreshCurrentCoreId();
            }

            return currentProcessorIdCache >> CacheShift;
        }

        /// <summary>
        /// Consider flushing the currentProcessorIdCache on Wait operations or similar
        /// actions that are likely to result in changing the executing core.
        /// </summary>
        public static void FlushCurrentCpuId()
        {
            _currentProcessorIdCache &= (~CacheCountDownMask) | 1;
        }
    }

    internal static class CpuIdExtensions
    {
        // TODO (review) instead of using everywhere via e.g. such extension
        // methods should use Flush directly where kernel wait is likely.
        // Awaiting tasks does not require flushing, with async we should never 
        // block. But waiting a task is blocking.
        [Obsolete]
        public static void WaitFlushProcessorId(this SemaphoreSlim semaphoreSlim)
        {
            // this two-step waiting breaks fairness
            // var result = semaphoreSlim.Wait(0);
            // if(result)
            //     return;
            semaphoreSlim.Wait();
            Cpu.FlushCurrentCpuId();
        }
    }
}