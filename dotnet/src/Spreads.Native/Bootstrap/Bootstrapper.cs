// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Spreads.Native.Bootstrap
{
    public class Bootstrapper
    {
        // ReSharper disable once InconsistentNaming
        internal static ABI ABI { get; set; }

        internal static Bootstrapper Instance { get; } = new Bootstrapper();

        static Bootstrapper()
        {
            ABI = Process.DetectABI();
        }

        private const string ConfigSubFolder = "config";
        private const string BinSubFolder = "bin";
        private const string DataSubFolder = "data";

        private Bootstrapper()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public static string DefaultAppName { get; set; }

        public string AppFolder { get; internal set; }

        public string _configFolder;
        public string ConfigFolder => _configFolder ?? (_configFolder = Path.Combine(AppFolder, ConfigSubFolder));

        public string _binFolder;
        public string BinFolder => _binFolder ?? (_binFolder = Path.Combine(AppFolder, BinSubFolder));

        public string _dataFolder;
        public string DataFolder => _dataFolder ?? (_dataFolder = Path.Combine(AppFolder, DataSubFolder));

        internal string _tempFolder;

        public string TempFolder
        {
            get
            {
                if (_tempFolder != null)
                {
                    return _tempFolder;
                }
                // on-demand creation on first access to TempFolder
                _tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(_tempFolder);
                return _tempFolder;
            }
        }

        internal Dictionary<string, NativeLibrary> NativeLibraries = new Dictionary<string, NativeLibrary>();

        private readonly List<Action> _disposeActions = new List<Action>();
        private bool _initialized;

        /// <summary>
        /// If <paramref name="appName"/> if provided then <see cref="AppFolder"/> is set to
        /// <paramref name="appName"/> inside <see cref="Environment.SpecialFolder.LocalApplicationData"/>.
        /// If <paramref name="appFolderPath"/> is provided then <see cref="AppFolder"/>  is set to it.
        /// One one of the parameters could be not null.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appFolderPath"></param>
        public static void Init(string appName = null, string appFolderPath = null)
        {
            if (Instance._initialized)
            {
                return;
            }
            if (appName != null && appFolderPath != null && !appFolderPath.EndsWith(appName))
            {
                throw new InvalidOperationException("Either appName or appFolderPath should be provided, not both.");
            }

            if (appName == null && appFolderPath == null)
            {
                if (!string.IsNullOrWhiteSpace(DefaultAppName))
                {
                    appName = DefaultAppName;
                }
                else
                {
                    Instance._initialized = true;
                    return;
                }
                
            }

            if (appFolderPath != null)
            {
                Directory.CreateDirectory(appFolderPath);
                Instance.AppFolder = appFolderPath;
            }

            if (appName != null)
            {
                Instance.AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DefaultAppName);
            }

            // if exists it's noop
            Directory.CreateDirectory(Instance.BinFolder);
            Directory.CreateDirectory(Instance.ConfigFolder);
            Directory.CreateDirectory(Instance.DataFolder);

            Instance._initialized = true;
        }

        /// <summary>
        /// Load libraries from an assembly with type <typeparamref name="T"/>.
        /// Executes only once. Calls after the first one are noops.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nativeLibraryName"></param>
        /// <param name="preLoadAction"></param>
        /// <param name="postLoadAction"></param>
        /// <param name="disposeAction"></param>
        /// <param name="nativeLoadPathOverride">
        /// Path to a directory with the native library. On Linux DllImport won't work with a custom path
        /// and you could setup delegates to native methods from <paramref name="postLoadAction"/>.</param>
        public static void Bootstrap<T>(string nativeLibraryName,
            Action<Bootstrapper> preLoadAction = null,
            Action<NativeLibrary> postLoadAction = null,
            Action disposeAction = null,
            string nativeLoadPathOverride = null)
        {
            if (!Instance._initialized)
            {
                Init();
            }

            if (TypeCache<T>.Done)
            {
                return;
            }

            preLoadAction?.Invoke(Instance);

            NativeLibrary nativeLibrary = null;
            if (nativeLibraryName != null)
            {
                if (!Instance.NativeLibraries.ContainsKey(nativeLibraryName))
                {
                    nativeLibrary = Loader.LoadNativeLibrary<T>(nativeLibraryName, nativeLoadPathOverride);
                    Instance.NativeLibraries.Add(nativeLibraryName, nativeLibrary);
                    Trace.TraceInformation("Loaded native library: " + nativeLibraryName);
                }
            }

            postLoadAction?.Invoke(nativeLibrary);

            Instance._disposeActions.Add(disposeAction);

            TypeCache<T>.Done = true;
        }

        ~Bootstrapper()
        {
            if (_disposeActions.Count > 0)
            {
                foreach (var action in _disposeActions)
                {
                    try
                    {
                        action.Invoke();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
            }

            foreach (var loadedLibrary in NativeLibraries)
            {
                if (loadedLibrary.Value != null)
                {
                    try
                    {
                        loadedLibrary.Value.Dispose();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
            }

            try
            {
                if (Instance._tempFolder != null)
                {
                    Directory.Delete(Instance._tempFolder, true);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        // ReSharper disable once UnusedTypeParameter
        private static class TypeCache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static bool Done;
        }
    }
}
