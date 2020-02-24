// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Spreads.Native
{
    /// TODO check actual size
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RuntimeVecInfo
    {
        public Type Type;
        internal IntPtr UnsafeGetterPtr;
        internal IntPtr UnsafeSetterPtr;
        public int RuntimeTypeId;
        public short ElemSize;
        public int ArrayOffsetAdjustment;
        public bool IsReferenceOrContainsReferences;
    }

    internal static class VecTypeHelper
    {
        // this is basically a manual vtable for a particular use case
        // cannot make untyped delegates to perform at least on par with indirect calls
        private static readonly AppendOnlyStorage<RuntimeVecInfo> Info = new AppendOnlyStorage<RuntimeVecInfo>();

        private static readonly ConcurrentDictionary<Type, int> Lookup = new ConcurrentDictionary<Type, int>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref RuntimeVecInfo GetInfo(Type ty)
        {
            var idx = Lookup.GetOrAdd(ty, tNew =>
            {
                // this lambda is not thread safe
                lock (Info)
                {
                    if (Lookup.TryGetValue(tNew, out var idx2))
                    {
                        return idx2;
                    }

                    foreach (var runtimeVecInfo in Info._storage)
                    {
                        if (runtimeVecInfo.Type != null && runtimeVecInfo.Type == tNew)
                        {
                            return runtimeVecInfo.RuntimeTypeId;
                        }
                    }

                    var idxNew = Info.Add(new RuntimeVecInfo()
                    {
                        Type = tNew,
                        UnsafeGetterPtr = UnsafeEx.GetterMethodPointerForType(tNew),
                        UnsafeSetterPtr = UnsafeEx.SetterMethodPointerForType(tNew),
                        ElemSize = checked((short) UnsafeEx.ElemSizeOfType(tNew)),
                        ArrayOffsetAdjustment = UnsafeEx.ArrayOffsetAdjustmentOfType(tNew),
                        IsReferenceOrContainsReferences = IsReferenceOrContainsReferences(tNew)
                    });
                    ref var infoNew = ref Info[idxNew];
                    infoNew.RuntimeTypeId = idxNew;
                    return idxNew;
                }
            });
            return ref Info[idx];
        }

        internal static bool IsReferenceOrContainsReferences(Type type)
        {
#if HAS_ISREF
            var method = typeof(RuntimeHelpers).GetMethod("IsReferenceOrContainsReferences", BindingFlags.Static | BindingFlags.Public);
            // ReSharper disable once PossibleNullReferenceException
            var genericMethod = method!.MakeGenericMethod(type);
            return (bool) genericMethod.Invoke(null, null)!;
#else
            return IsReferenceOrContainsReferencesManual(type);
#endif
            
        }

        internal static bool IsReferenceOrContainsReferencesManual(Type type)
        {
            if (type.GetTypeInfo().IsPrimitive) // This is hopefully the common case. All types that return true for this are value types w/out embedded references.
                return false;

            if (!type.GetTypeInfo().IsValueType)
                return true;

            // If type is a Nullable<> of something, unwrap it first.
            Type underlyingNullable = Nullable.GetUnderlyingType(type);
            if (underlyingNullable != null)
                type = underlyingNullable;

            if (type.GetTypeInfo().IsEnum)
                return false;

            foreach (FieldInfo field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.IsStatic)
                    continue;
                if (IsReferenceOrContainsReferences(field.FieldType))
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref RuntimeVecInfo GetInfo(int runtimeTypeId)
        {
            return ref Info[runtimeTypeId];
        }

        /// <summary>
        /// A helper class to store types info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal class AppendOnlyStorage<T>
        {
            internal T[] _storage = new T[16];
            private int _counter;

            public int Add(T value)
            {
                lock (this)
                {
                    // could use ++ if locked
                    var cnt = Interlocked.Increment(ref _counter);
                    var idx = cnt - 1;
                    if (_counter > _storage.Length)
                    {
                        var newStorage = new T[_storage.Length * 2];
                        _storage.CopyTo(newStorage, 0);
                        _storage = newStorage; // ref assignment is atomic
                    }

                    _storage[idx] = value;
                    return cnt;
                }
            }

            public ref T this[int index]
            {
                // no locks here because _storage could be changed atomically
                // and to use an index from code it must be added first and
                // Add must return first (otherwise usage is broken).
                // No range check because with correct usage it's always in range
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _storage[index - 1];
            }
        }
    }

    public static class VecTypeHelper<T>
    {
        internal static readonly T[] ArrayWithTwoElements = new T[2];
        
        private static readonly RuntimeVecInfo RuntimeVecInfo = VecTypeHelper.GetInfo(typeof(T));

        // ReSharper disable once StaticMemberInGenericType
        public static readonly int RuntimeTypeId = RuntimeVecInfo.RuntimeTypeId;
        public static readonly bool IsReferenceOrContainsReferences = RuntimeVecInfo.IsReferenceOrContainsReferences;
        public static readonly int ArrayOffsetAdjustment = RuntimeVecInfo.ArrayOffsetAdjustment;

        // Array header sizes are a runtime implementation detail and aren't the same across all runtimes. (The CLR made a tweak after 4.5, and Mono has an extra Bounds pointer.)
        internal static IntPtr MeasureArrayAdjustment()
        {
            return Unsafe.ByteOffset(ref Unsafe.As<Pinnable<T>>(ArrayWithTwoElements).Data, ref ArrayWithTwoElements[0]);
        }
    }
}