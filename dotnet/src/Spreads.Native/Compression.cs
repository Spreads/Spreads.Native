// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Spreads.Native.Bootstrap;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles

namespace Spreads.Native
{
    /// <summary>
    /// Native compression methods.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public unsafe class Compression
    {
        internal const string NativeLibraryName = "spreads_native";

        internal static IntPtr compress_lz4_ptr;
        internal static IntPtr decompress_lz4_ptr;

        internal static IntPtr compress_zstd_ptr;
        internal static IntPtr decompress_zstd_ptr;

        internal static IntPtr compress_zlib_ptr;
        internal static IntPtr decompress_zlib_ptr;

        internal static IntPtr compress_deflate_ptr;
        internal static IntPtr decompress_deflate_ptr;

        internal static IntPtr compress_gzip_ptr;
        internal static IntPtr decompress_gzip_ptr;

        internal static IntPtr shuffle_ptr;
        internal static IntPtr unshuffle_ptr;

        internal static IntPtr compress_copy_ptr = UnsafeEx.CopyCompressMethod();
        internal static IntPtr decompress_copy_ptr = UnsafeEx.CopyDecompressMethod();

        static Compression()
        {
            try
            {
                // Ensure Bootstrapper is initialized and native libraries are loaded
                Bootstrapper.Bootstrap<Compression>(
                    NativeLibraryName,
                    instance =>
                    {
                    },
                    library =>
                    {
                        compress_lz4_ptr = library.GetFunctionPtr("spreads_compress_lz4");
                        decompress_lz4_ptr = library.GetFunctionPtr("spreads_decompress_lz4");

                        compress_zstd_ptr = library.GetFunctionPtr("spreads_compress_zstd");
                        decompress_zstd_ptr = library.GetFunctionPtr("spreads_decompress_zstd");

                        compress_zlib_ptr = library.GetFunctionPtr("spreads_compress_zlib");
                        decompress_zlib_ptr = library.GetFunctionPtr("spreads_decompress_zlib");

                        compress_deflate_ptr = library.GetFunctionPtr("spreads_compress_deflate");
                        decompress_deflate_ptr = library.GetFunctionPtr("spreads_decompress_deflate");

                        compress_gzip_ptr = library.GetFunctionPtr("spreads_compress_gzip");
                        decompress_gzip_ptr = library.GetFunctionPtr("spreads_decompress_gzip");

                        shuffle_ptr = library.GetFunctionPtr("spreads_shuffle");
                        unshuffle_ptr = library.GetFunctionPtr("spreads_unshuffle");
                    },
                    () => { });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error in BloscMethods Init: {ex}");
                throw;
            }
        }

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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void shuffle(IntPtr typeSize, IntPtr length, byte* source, byte* destination)
        //{
        //    UnsafeEx.CalliShuffleUnshuffle(typeSize, length, source, destination, shuffle_ptr);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void unshuffle(IntPtr typeSize, IntPtr length, byte* source, byte* destination)
        //{
        //    UnsafeEx.CalliShuffleUnshuffle(typeSize, length, source, destination, unshuffle_ptr);
        //}
    }
}
