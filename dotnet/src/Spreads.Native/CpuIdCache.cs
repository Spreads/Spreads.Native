// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Spreads.Native
{
    [SuppressUnmanagedCodeSecurity]
    public static class CpuIdCache
    {
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

        private const int RefreshRate = 500;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RefreshCurrentCpuId()
        {
            int currentProcessorId = get_cpu_number();

            // On Unix, GetCurrentProcessorNumber() is implemented in terms of sched_getcpu, which
            // doesn't exist on all platforms.  On those it doesn't exist on, GetCurrentProcessorNumber()
            // returns -1.  As a fallback in that case and to spread the threads across the buckets
            // by default, we use the current managed thread ID as a proxy.
            if (currentProcessorId < 0)
                currentProcessorId = Environment.CurrentManagedThreadId;

            // Add offset to make it clear that it is not guaranteed to be 0-based processor number
            currentProcessorId += 100;

            // Mask with int.MaxValue to ensure the execution Id is not negative
            _currentProcessorIdCache = ((currentProcessorId << CacheShift) & int.MaxValue) |
                                       RefreshRate;

            return currentProcessorId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCurrentCpuId()
        {
            int currentProcessorIdCache = _currentProcessorIdCache--;
            if ((currentProcessorIdCache & CacheCountDownMask) == 0)
            {
                return RefreshCurrentCpuId();
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
            CpuIdCache.FlushCurrentCpuId();
        }
    }
}