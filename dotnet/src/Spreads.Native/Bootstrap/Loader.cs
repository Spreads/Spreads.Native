// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Spreads.Native.Bootstrap
{
    // TODO internal
    internal interface INativeLibraryLoader
    {
        IntPtr LoadLibrary(string path);

        bool UnloadLibrary(IntPtr library);

        IntPtr FindFunction(IntPtr library, string function);

        IntPtr LastError();
    }

    internal sealed class WindowsLibraryLoader : INativeLibraryLoader
    {
        IntPtr INativeLibraryLoader.LoadLibrary(string path)
        {
            return LoadLibraryW(path);
        }

        bool INativeLibraryLoader.UnloadLibrary(IntPtr library)
        {
            return FreeLibrary(library);
        }

        IntPtr INativeLibraryLoader.FindFunction(IntPtr library, string function)
        {
            return GetProcAddress(library, function);
        }

        public IntPtr LastError()
        {
            return GetLastError();
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr LoadLibraryW(string path);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr library);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr library, string function);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetLastError();
    }

    internal abstract class UnixLibraryLoader : INativeLibraryLoader
    {
        IntPtr INativeLibraryLoader.LoadLibrary(string path)
        {
            Trace.WriteLine("Opening a library: " + path);
            try
            {
                int flags = GetDLOpenFlags();
                var result = dlopen(path, flags);
                Trace.WriteLine("Open result: " + result);
                if (result == IntPtr.Zero)
                {
                    var lastError = dlerror();
                    Trace.WriteLine($"Failed to load native library \"{path}\".\r\nLast Error:{lastError}\r\nCheck inner exception and\\or windows event log.");
                }
                return result;
            }
            catch (Exception ex)
            {
                var lastError = dlerror();
                Trace.WriteLine($"Failed to load native library \"{path}\".\r\nLast Error:{lastError}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {ex}");

                Trace.WriteLine(ex.ToString());
                return IntPtr.Zero;
            }
        }

        bool INativeLibraryLoader.UnloadLibrary(IntPtr library)
        {
            return dlclose(library) == 0;
        }

        IntPtr INativeLibraryLoader.FindFunction(IntPtr library, string function)
        {
            return dlsym(library, function);
        }

        protected abstract int GetDLOpenFlags();

        private static IntPtr dlopen(string path, int flags)
        {
            try
            {
                return UnixLibraryLoaderNative.dlopen(path, flags);
            }
            catch
            {
                return UnixLibraryLoaderNative2.dlopen(path, flags);
            }
        }

        private static IntPtr dlsym(IntPtr library, string function)
        {
            try
            {
                return UnixLibraryLoaderNative.dlsym(library, function);
            }
            catch
            {
                return UnixLibraryLoaderNative2.dlsym(library, function);
            }
        }

        private static int dlclose(IntPtr library)
        {
            try
            {
                return UnixLibraryLoaderNative.dlclose(library);
            }
            catch
            {
                return UnixLibraryLoaderNative2.dlclose(library);
            }
        }

        private static IntPtr dlerror()
        {
            try
            {
                return UnixLibraryLoaderNative.dlerror();
            }
            catch
            {
                return UnixLibraryLoaderNative2.dlerror();
            }
        }

        public IntPtr LastError()
        {
            return dlerror();
        }

        private static class UnixLibraryLoaderNative
        {
            [DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

            [DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlsym(IntPtr library, [MarshalAs(UnmanagedType.LPStr)] string function);

            [DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
            public static extern int dlclose(IntPtr library);

            [DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlerror();
        }

        // see https://github.com/dotnet/corefx/issues/17135
        // for FreeBSD may need another option with 'libc'
        private static class UnixLibraryLoaderNative2
        {
            [DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

            [DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlsym(IntPtr library, [MarshalAs(UnmanagedType.LPStr)] string function);

            [DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl)]
            public static extern int dlclose(IntPtr library);

            [DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr dlerror();
        }
    }

    internal sealed class AndroidLibraryLoader : UnixLibraryLoader
    {
        protected override int GetDLOpenFlags()
        {
            return RTLD_NOW | RTLD_LOCAL;
        }

        private const int RTLD_NOW = 0x00000000;
        private const int RTLD_LOCAL = 0x00000000;
    }

    internal sealed class LinuxLibraryLoader : UnixLibraryLoader
    {
        protected override int GetDLOpenFlags()
        {
            return RTLD_NOW | RTLD_LOCAL;
        }

        private const int RTLD_NOW = 0x00000002;
        private const int RTLD_LOCAL = 0x00000000;
    }

    internal sealed class OSXLibraryLoader : UnixLibraryLoader
    {
        protected override int GetDLOpenFlags()
        {
            return RTLD_NOW | RTLD_LOCAL;
        }

        private const int RTLD_NOW = 0x00000002;
        private const int RTLD_LOCAL = 0x00000004;
    }

    internal class Loader
    {
        public static NativeLibrary LoadNativeLibrary<T>(string libName, string nativeLoadPathOverride = null)
        {
            var abi = Process.DetectABI();
            if (abi.Equals(ABI.Unknown))
            { return null; }

            var loader = GetNativeLibraryLoader(abi);
            if (loader == null)
            { return null; }

            var nativeAssemblyPath = nativeLoadPathOverride ?? Path.GetDirectoryName(typeof(T).GetTypeInfo().Assembly.Location);
            if (nativeAssemblyPath is null)
            {
                throw new IOException("nativeAssemblyPath is null");
            }
            string libPath = null;
            var localExists = false;

            // We need x86 only on Windows due to WoW64 that we actually use in certain situations.
            //
            // On Windows, LoadLibrary makes it always available for DllImport regardless of the location.
            //
            // On Linux, we need to set LD_LIBRARY_PATH which is not possible to do from
            // already running app that needs it. But we do not care about x86 Linux at the moment
            // and keep native library at the app root.
            //
            // If we start care then we should copy a relevant lib to the app root. There is no WoW64-like
            // stuff on Linux and after the first copy the file should always be correct.
            //
            // MacOS is only x64 and a native lib should be at the app root.

            if (abi.Equals(ABI.Windows_X86_64) &&
                    (File.Exists(libPath = Path.Combine(nativeAssemblyPath, libName + (libName.EndsWith(".dll") ? "" : ".dll")))
                    ||
                    File.Exists(libPath = Path.Combine(nativeAssemblyPath, "x64", libName + (libName.EndsWith(".dll") ? "" : ".dll")))
                    ))
            {
                localExists = true;
            }
            else if (abi.Equals(ABI.Windows_X86) &&
                     (File.Exists(libPath = Path.Combine(nativeAssemblyPath, "x86", libName + (libName.EndsWith(".dll") ? "" : ".dll")))
                      ||
                      File.Exists(libPath = Path.Combine(nativeAssemblyPath, "x86", libName + (libName.EndsWith(".dll") ? "" : ".dll")))
                     ))
            {
                localExists = true;
            }
            else if (abi.Equals(ABI.Linux_X86_64) &&
                     (File.Exists(libPath = Path.Combine(nativeAssemblyPath, libName + (libName.EndsWith(".so") ? "" : ".so")))
                      ||
                      File.Exists(libPath = Path.Combine(nativeAssemblyPath, "lib" + libName + (libName.EndsWith(".so") ? "" : ".so")))
                     ))
            {
                localExists = true;
            }
            else if (abi.Equals(ABI.OSX_X86_64) &&
                     (File.Exists(libPath = Path.Combine(nativeAssemblyPath, libName + (libName.EndsWith(".dylib") ? "" : ".dylib")))
                      ||
                      File.Exists(libPath = Path.Combine(nativeAssemblyPath, "lib" + libName + (libName.EndsWith(".dylib") ? "" : ".dylib")))
                     ))
            {
                localExists = true;
            }

            if (localExists)
            {
                var handle = loader.LoadLibrary(libPath);
                if (handle != IntPtr.Zero)
                {
                    return new NativeLibrary(libPath, loader, handle);
                }
            }

            throw new DllNotFoundException("Cannot find native library: " + libName);
        }

        public static INativeLibraryLoader GetNativeLibraryLoader(ABI abi)
        {
            if (abi.IsWindows())
            { return new WindowsLibraryLoader(); }
            if (abi.IsLinux())
            { return new LinuxLibraryLoader(); }
            if (abi.IsOSX())
            { return new OSXLibraryLoader(); }

            return null;
        }
    }
}
