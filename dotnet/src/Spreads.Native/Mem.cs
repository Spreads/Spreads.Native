// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable UnusedMember.Global

namespace Spreads.Native
{
    [SuppressUnmanagedCodeSecurity]
    public class Mem
    {
        private const string NativeLibraryName = Constants.NativeLibraryName;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public unsafe delegate void DeferredFreeFun(bool force, ulong heartbeat, void* arg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public unsafe delegate void OutputFun([MarshalAs(UnmanagedType.LPStr)] string msg, void* arg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public unsafe delegate void ErrorFun(int err, void* arg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public unsafe delegate bool BlockVisitFun(void* heap, void* area, void* block, UIntPtr blockSize, void* arg);

        public enum Option
        {
            // stable options
            ShowSrrors,
            ShowStats,
            OptionVerbose,

            // the following options are experimental
            EagerCommit,
            EagerRegionCommit,
            ResetDecommits,
            LargeOsPages, // implies eager commit
            ReserveHugeOsPages,
            SegmentCache,
            PageReset,
            AbandonedPageReset,
            SegmentReset,
            EagerCommitDelay,
            ResetDelay,
            UseNumaNodes,
            OsTag,
            MaxErrors,
            Last,
            EagerPageCommit = EagerCommit
        }

        /// <summary>
        /// Allocate zero-initialized <paramref name="count"/> elements of <paramref name="size"/> bytes.
        /// Returns a pointer to the allocated memory of <paramref name="count"/>*<paramref name="size"/> bytes, or NULL if either out of memory or when <paramref name="count"/>*<paramref name="size"/> overflows.
        /// </summary>
        /// <param name="count">The number of elements.</param>
        /// <param name="size">The size of each element.</param>
        /// <seealso cref="Mallocn"/>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Calloc(UIntPtr count, UIntPtr size);

        /// <summary>
        /// Allocate <paramref name="size"/> bytes. Returns a pointer to the allocated memory or NULL if out of memory. Returns a unique pointer if called with size 0.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Malloc(UIntPtr size);

        /// <summary>
        /// Re-allocate memory to <paramref name="newsize"/> bytes.
        /// Returns a pointer to the re-allocated memory of newsize bytes, or NULL if out of memory.
        /// If NULL is returned, the pointer p is not freed. Otherwise the original pointer is either
        /// freed or returned as the reallocated result (in case it fits in-place with the new size).
        /// If the pointer p is NULL, it behaves as mi_malloc(newsize). If newsize is larger than the
        /// original size allocated for p, the bytes after size are uninitialized.
        /// </summary>
        /// <param name="p">pointer to previously allocated memory (or NULL).</param>
        /// <param name="newsize">the new required size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_realloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Realloc(byte* p, UIntPtr newsize);

        /// <summary>
        /// Free previously allocated memory. The pointer p must have been allocated before (or be NULL).
        /// </summary>
        /// <param name="p">pointer to free, or NULL.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void Free(byte* p);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc_small",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* MallocSmall(UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_small",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ZallocSmall(UIntPtr size);

        /// <summary>
        /// Allocate zero-initialized size bytes. Returns a pointer to newly allocated zero initialized memory, or NULL if out of memory.
        /// </summary>
        /// <param name="size">The size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Zalloc(UIntPtr size);

        /// <summary>
        /// Allocate count elements of size bytes.
        /// Returns a pointer to a block of <paramref name="count"/>*<paramref name="size"/>  bytes, or NULL if out of memory or if <paramref name="count"/>*<paramref name="size"/>  overflows.
        /// If there is no overflow, it behaves exactly like <see cref="Malloc"/> with memory size = <paramref name="count"/>*<paramref name="size"/>.
        /// </summary>
        /// <param name="count">The number of elements.</param>
        /// <param name="size">The size of each element.</param>
        /// <seealso cref="Calloc"/>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_mallocn", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Mallocn(UIntPtr count, UIntPtr size);

        /// <summary>
        /// Re-allocate memory to count elements of size bytes.
        /// Returns a pointer to a re-allocated block of <paramref name="count"/>*<paramref name="size"/> bytes, or NULL if out of memory or if <paramref name="count"/>*<paramref name="size"/> overflows.
        /// If there is no overflow, it behaves exactly like Realloc(p,count*size).
        /// </summary>
        /// <param name="p">Pointer to a previously allocated block (or NULL).</param>
        /// <param name="count">The number of elements.</param>
        /// <param name="size">The size of each element.</param>
        /// <seealso cref="Realloc"/>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_reallocn", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Reallocn(byte* p, UIntPtr count, UIntPtr size);

        /// <summary>
        /// Re-allocate memory to <paramref name="newsize"/> bytes.
        /// Returns a pointer to the re-allocated memory of <paramref name="newsize"/>  bytes, or NULL if out of memory.
        /// In contrast to <see cref="Realloc"/>, if NULL is returned, the original pointer p is freed (if it was not NULL itself).
        /// Otherwise the original pointer is either freed or returned as the reallocated result (in case it fits in-place with the new size).
        /// If the pointer p is NULL, it behaves as <see cref="Malloc"/> with <paramref name="newsize"/>.
        /// If <paramref name="newsize"/> is larger than the original size allocated for <paramref name="p"/>, the bytes after size are uninitialized.
        /// </summary>
        /// <param name="p">pointer to previously allocated memory (or NULL).</param>
        /// <param name="newsize">	the new required size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_reallocf", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Reallocf(byte* p, UIntPtr newsize);

        /// <summary>
        /// Try to re-allocate memory to <paramref name="newsize"/>  bytes in place.
        /// Returns a pointer to the re-allocated memory of <paramref name="newsize"/>
        /// bytes (always equal to <paramref name="p"/> ), or NULL if either out of memory
        /// or if the memory could not be expanded in place. If NULL is returned,
        /// the pointer <paramref name="p"/>  is not freed. Otherwise the original pointer is returned
        /// as the reallocated result since it fits in-place with the new size. If <paramref name="newsize"/>
        /// is larger than the original size allocated for <paramref name="p"/> , the bytes after size are uninitialized.
        /// </summary>
        /// <param name="p">pointer to previously allocated memory (or NULL).</param>
        /// <param name="newsize">the new required size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_expand",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Expand(byte* p, UIntPtr newsize);

        /// <summary>
        /// Returns the available bytes in the memory block, or 0 if p was NULL.
        /// The returned size can be used to call <see cref="Expand"/> successfully.
        /// The returned size is always at least equal to the allocated size of p, and, in the current design, should be less than 16.7% more.
        /// </summary>
        /// <param name="p">Pointer to previously allocated memory (or NULL)</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_usable_size",
            CallingConvention = CallingConvention.Cdecl)]
#if NET5_0
        [SuppressGCTransition]
#endif
        public static extern unsafe UIntPtr UsableSize(byte* p);

        /// <summary>
        /// Returns the size n that will be allocated, where n >= size.
        /// Generally, UsableSize(Malloc(size)) == GoodSize(size).
        /// This can be used to reduce internal wasted space when allocating buffers for example.
        /// </summary>
        /// <param name="size">The minimal required size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_good_size",
            CallingConvention = CallingConvention.Cdecl)]
#if NET5_0
        [SuppressGCTransition]
#endif
        public static extern UIntPtr GoodSize(UIntPtr size);

        /// <summary>
        /// Eagerly free memory. Regular code should not have to call this function.
        /// It can be beneficial in very narrow circumstances; in particular,
        /// when a long running thread allocates a lot of blocks that are freed by
        /// other threads it may improve resource usage by calling this every once in a while.
        /// </summary>
        /// <param name="force">If true, aggressively return memory to the OS (can be expensive!).</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_collect", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Collect(bool force);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_mialloc_version",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int MimallocVersion();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* MallocAligned(UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* MallocAlignedAt(UIntPtr size, UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ZallocAligned(UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ZallocAlignedAt(UIntPtr size, UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* CallocAligned(UIntPtr count, UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* CallocAlignedAt(UIntPtr count, UIntPtr size, UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_realloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ReallocAligned(byte* p, UIntPtr newsize, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_realloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ReallocAlignedAt(byte* p, UIntPtr newsize, UIntPtr alignment,
            UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Rezalloc(byte* p, UIntPtr newsize);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* Recalloc(byte* p, UIntPtr newcount, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* RezallocAligned(byte* p, UIntPtr newsize, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* RezallocAlignedAt(byte* p, UIntPtr newsize, UIntPtr alignment,
            UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* RecallocAligned(byte* p, UIntPtr newcount, UIntPtr size,
            UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* RecallocAlignedAt(byte* p, UIntPtr newcount, UIntPtr size,
            UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_stats_reset",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatsReset();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_stats_merge",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatsMerge();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_stats_print",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatsPrint();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_stats_print_out",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void StatsPrintOut(OutputFun outputFun, void* arg);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_register_deferred_free",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void RegisterDeferredFree(DeferredFreeFun deferredFree, void* arg);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_register_output",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void RegisterOutput(OutputFun outputFun, void* arg);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_register_error",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void RegisterError(OutputFun outputFun, void* arg);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_is_enabled",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool OptionIsEnabled(Option option);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_enable",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionEnable(Option option);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_disable",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionDisable(Option option);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_set_enabled",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionSetEnabled(Option option, bool enabled);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_set_enabled_default",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionSetEnabledDefault(Option option, bool enabled);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_get",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int OptionGet(Option option);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_set",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionSet(Option option, int value);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_option_set_default",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptionSetDefault(Option option, int value);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_new",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* HeapNew();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_delete",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void HeapDelete(void* heap);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_destroy",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void HeapDestroy(void* heap);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_set_default",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* HeapSetDefault(void* heap);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_get_default",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* HeapGetDefault();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_get_backing",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* HeapGetBacking();

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_visit_blocks",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe bool HeapVisitBlocks(void* heap, bool visitAllBlocks, BlockVisitFun visitor,
            void* arg);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_is_in_heap_region",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe bool IsInHeapRegion(byte* p);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_collect",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void HeapCollect(void* heap, bool force);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_malloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapMalloc(void* heap, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_zalloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapZalloc(void* heap, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_calloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapCalloc(void* heap, UIntPtr count, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_mallocn",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapMallocn(void* heap, UIntPtr count, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_malloc_small",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapMallocSmall(void* heap, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_realloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRealloc(void* heap, byte* p, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_reallocn",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapReallocn(void* heap, byte* p, UIntPtr count, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_reallocf",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapReallocf(void* heap, byte* p, UIntPtr newsize);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_strdup",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapStrdup(void* heap, byte* s);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_strndup",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapStrndup(void* heap, byte* s, UIntPtr n);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_realpath",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRealpath(void* heap, byte* fname, byte* resolvedName);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_malloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapMallocAligned(void* heap, UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_malloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapMallocAlignedAt(void* heap, UIntPtr size, UIntPtr alignment,
            UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_zalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapZallocAligned(void* heap, UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_zalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapZallocAlignedAt(void* heap, UIntPtr size, UIntPtr alignment,
            UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_calloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapCallocAligned(void* heap, UIntPtr count, UIntPtr size,
            UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_calloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapCallocAlignedAt(void* heap, UIntPtr count, UIntPtr size,
            UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_realloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapReallocAligned(void* heap, byte* p, UIntPtr newsize,
            UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_realloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapReallocAlignedAt(void* heap, byte* p, UIntPtr newsize,
            UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_rezalloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRezalloc(void* heap, byte* p, UIntPtr newsize);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_recalloc",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRecalloc(void* heap, byte* p, UIntPtr newcount, UIntPtr size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_rezalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRezallocAligned(void* heap, byte* p, UIntPtr newsize,
            UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_rezalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRezallocAlignedAt(void* heap, byte* p, UIntPtr newsize,
            UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_recalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRecallocAligned(void* heap, byte* p, UIntPtr newcount,
            UIntPtr size, UIntPtr alignment);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_recalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* HeapRecallocAlignedAt(void* heap, byte* p, UIntPtr newcount,
            UIntPtr size, UIntPtr alignment, UIntPtr offset);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_contains_block",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe bool HeapContainsBlock(void* heap, byte* p);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_heap_check_owned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe bool HeapCheckOwned(void* heap, byte* p);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_process_info",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool ProcessInfo(nuint* elapsedMsecs,
            nuint* userMsecs,
            nuint* systemMsecs,
            nuint* currentRss,
            nuint* peakRss,
            nuint* currentCommit,
            nuint* peakCommit,
            nuint* pageFaults);

        public static unsafe MemProcessInfo ProcessInfo()
        {
            MemProcessInfo result = default;
            ProcessInfo(
                &result.ElapsedMsecs,
                &result.UserMsecs,
                &result.SystemMsecs,
                &result.CurrentRss,
                &result.PeakRss,
                &result.CurrentCommit,
                &result.PeakCommit,
                &result.PageFaults
            );
            return result;
        }

        [DebuggerDisplay("{ToString()}")]
        public readonly  struct MemProcessInfo
        {
            public readonly nuint ElapsedMsecs;
            public readonly nuint UserMsecs;
            public readonly nuint SystemMsecs;
            public readonly nuint CurrentRss;
            public readonly nuint PeakRss;
            public readonly nuint CurrentCommit;
            public readonly nuint PeakCommit;
            public readonly nuint PageFaults;

            public override string ToString()
            {
                return $"ElapsedMsecs: {(long)ElapsedMsecs:N}, UserMsecs: {(long)UserMsecs:N}, SystemMsecs: {(long)SystemMsecs:N}, " +
                       $"CurrentRss: {(long)CurrentRss:N}, PeakRss: {(long)PeakRss:N}, CurrentCommit: {(long)CurrentCommit:N}, " +
                       $"PeakCommit: {(long)PeakCommit:N}, PageFaults: {(long)PageFaults:N}";
            }
        }
    }
}
