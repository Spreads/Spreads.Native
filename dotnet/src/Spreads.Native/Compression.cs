// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable All

#pragma warning disable IDE1006 // Naming Styles

namespace Spreads.Native
{
    /// <summary>
    /// Native compression methods.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public unsafe class Compression
    {
        internal const string NativeLibraryName = "libspreads_native";

        internal static IntPtr compress_copy_ptr = UnsafeEx.CopyCompressMethod();
        internal static IntPtr decompress_copy_ptr = UnsafeEx.CopyDecompressMethod();

        
        #region Blosc Internals

        [DllImport(NativeLibraryName, EntryPoint = "spreads_compress_lz4", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress_lz4(byte* source, IntPtr sourceLength,
                            byte* destination, IntPtr destinationLength, int clevel);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_decompress_lz4", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decompress_lz4(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_compress_zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress_zstd(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength, int clevel);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_decompress_zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decompress_zstd(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_compress_zlib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress_zlib(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength, int clevel);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_decompress_zlib", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decompress_zlib(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_compress_deflate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress_deflate(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength, int clevel);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_decompress_deflate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decompress_deflate(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_compress_gzip", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress_gzip(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength, int clevel);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_decompress_gzip", CallingConvention = CallingConvention.Cdecl)]
        public static extern int decompress_gzip(byte* source, IntPtr sourceLength,
            byte* destination, IntPtr destinationLength);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_shuffle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void shuffle(IntPtr typeSize, IntPtr length, byte* source, byte* destination);

        [DllImport(NativeLibraryName, EntryPoint = "spreads_unshuffle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void unshuffle(IntPtr typeSize, IntPtr length, byte* source, byte* destination);

        #endregion Blosc Internals
    }
}
