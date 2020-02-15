using System;
using System.Runtime.InteropServices;

namespace Spreads.Native
{
    public unsafe class Mem
    {
        private const string NativeLibraryName = UnsafeEx.NativeLibraryName;

        /// <summary>
        /// Allocate zero-initialized <paramref name="count"/> elements of <paramref name="size"/> bytes.
        /// Returns a pointer to the allocated memory of <paramref name="count"/>*<paramref name="size"/> bytes, or NULL if either out of memory or when <paramref name="count"/>*<paramref name="size"/> overflows.
        /// </summary>
        /// <param name="count">The number of elements.</param>
        /// <param name="size">The size of each element.</param>
        /// <seealso cref="Mallocn"/>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Calloc(uint count, uint size);

        /// <summary>
        /// Allocate <paramref name="size"/> bytes. Returns a pointer to the allocated memory or NULL if out of memory. Returns a unique pointer if called with size 0.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Malloc(uint size);

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
        public static extern byte* Realloc(byte* p, uint newsize);

        /// <summary>
        /// Free previously allocated memory. The pointer p must have been allocated before (or be NULL).
        /// </summary>
        /// <param name="p">pointer to free, or NULL.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Free(byte* p);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc_small",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* MallocSmall(uint size);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_small",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ZallocSmall(uint size);

        /// <summary>
        /// Allocate zero-initialized size bytes. Returns a pointer to newly allocated zero initialized memory, or NULL if out of memory.
        /// </summary>
        /// <param name="size">The size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Zalloc(uint size);

        /// <summary>
        /// Allocate count elements of size bytes.
        /// Returns a pointer to a block of <paramref name="count"/>*<paramref name="size"/>  bytes, or NULL if out of memory or if <paramref name="count"/>*<paramref name="size"/>  overflows.
        /// If there is no overflow, it behaves exactly like <see cref="Malloc"/> with memory size = <paramref name="count"/>*<paramref name="size"/>.
        /// </summary>
        /// <param name="count">The number of elements.</param>
        /// <param name="size">The size of each element.</param>
        /// <seealso cref="Calloc"/>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_mallocn", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Mallocn(uint count, uint size);

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
        public static extern byte* Reallocn(byte* p, uint count, uint size);

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
        public static extern byte* Reallocf(byte* p, uint newsize);


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
        public static extern byte* Expand(byte* p, uint newsize);


        /// <summary>
        /// Returns the available bytes in the memory block, or 0 if p was NULL.
        /// The returned size can be used to call <see cref="Expand"/> successfully.
        /// The returned size is always at least equal to the allocated size of p, and, in the current design, should be less than 16.7% more.
        /// </summary>
        /// <param name="p">Pointer to previously allocated memory (or NULL)</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_usable_size",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong UsableSize(byte* p);


        /// <summary>
        /// Returns the size n that will be allocated, where n >= size.
        /// Generally, UsableSize(Malloc(size)) == GoodSize(size).
        /// This can be used to reduce internal wasted space when allocating buffers for example.
        /// </summary>
        /// <param name="size">The minimal required size in bytes.</param>
        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_good_size",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GoodSize(uint size);

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
        public static extern byte* MallocAligned(uint size, uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_malloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* MallocAlignedAt(uint size, uint alignment, uint offset);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ZallocAligned(uint size, uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_zalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ZallocAlignedAt(uint size, uint alignment, uint offset);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* CallocAligned(uint count, uint size, uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_calloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* CallocAlignedAt(uint count, uint size, uint alignment, uint offset);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_realloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ReallocAligned(byte* p, uint newsize, uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_realloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ReallocAlignedAt(byte* p, uint newsize, uint alignment,
            uint offset);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Rezalloc(byte* p, uint newsize);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* Recalloc(byte* p, uint newcount, uint size);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* RezallocAligned(byte* p, uint newsize, uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_rezalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* RezallocAlignedAt(byte* p, uint newsize, uint alignment,
            uint offset);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc_aligned",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* RecallocAligned(byte* p, uint newcount, uint size,
            uint alignment);


        [DllImport(NativeLibraryName, EntryPoint = "spreads_mem_recalloc_aligned_at",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* RecallocAlignedAt(byte* p, uint newcount, uint size,
            uint alignment, uint offset);
    }
}