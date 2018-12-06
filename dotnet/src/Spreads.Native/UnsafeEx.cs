using System;
using System.Runtime.CompilerServices;

namespace Spreads
{
    public static unsafe class UnsafeEx
    {
        [MethodImpl(MethodImplOptions.ForwardRef)]
        private static extern int CopyWrapper(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        private static extern int CopyWrapper2(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr CopyCompressMethod();

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr CopyDecompressMethod();

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliCompress(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliCompressUnmanagedCdecl(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliDecompress(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliDecompressUnmanagedCdecl(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void CalliShuffleUnshuffle(IntPtr typeSize, IntPtr blockSize, void* source,
            void* destination, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrUintPptr(void* ptr1, void* ptr2, uint uint1, void** pptr1,
            IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrUintPtrPtr(void* ptr1, uint uint1, void* ptr2, void* ptr3,
            IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrUintPtrPtrUint(void* ptr1, uint uint1, void* ptr2, void* ptr3, uint uint2,
            IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void CalliVoidPtr(void* ptr1, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtr(void* ptr1, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtr(void* ptr1, void* ptr2, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrPtrInt(void* ptr1, void* ptr2, void* ptr3, int int1, IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrPtrUint(void* ptr1, void* ptr2, void* ptr3, uint uint1,
            IntPtr functionPtr);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CompareToConstrained<T>(ref T left, ref T right);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern bool EqualsConstrained<T>(ref T left, ref T right);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int GetHashCodeConstrained<T>(ref T obj);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void DisposeConstrained<T>(ref T obj);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T AddDeltaConstrained<T>(ref T obj, ref T delta);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T GetDeltaConstrained<T>(ref T obj, ref T other);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T AddLongConstrained<T>(ref T obj, long delta);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern long DiffLongConstrained<T>(ref T left, ref T right);
    }
}
