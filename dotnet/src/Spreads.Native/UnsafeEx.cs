// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable IdentifierTypo
#pragma warning disable 1584

namespace Spreads.Native
{
    /// <summary>
    /// Contains unsafe IL methods useful for Spreads.
    /// </summary>
    /// <seealso cref="System.Runtime.CompilerServices.Unsafe"/>
    public static unsafe class UnsafeEx
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        // ReSharper disable once UnusedMember.Local
        private static extern int CopyWrapper(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        // ReSharper disable once UnusedMember.Local
        private static extern int CopyWrapper2(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr CopyCompressMethod();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr CopyDecompressMethod();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliCompress(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliCompressUnmanagedCdecl(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, int clevel, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliDecompress(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliDecompressUnmanagedCdecl(void* source, IntPtr sourceLength, void* destination,
            IntPtr destinationLength, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void CalliShuffleUnshuffle(IntPtr typeSize, IntPtr blockSize, void* source,
            void* destination, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrUintPptr(void* ptr1, void* ptr2, uint uint1, void** pptr1,
            IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrUintPtrPtr(void* ptr1, uint uint1, void* ptr2, void* ptr3,
            IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrUintPtrPtrUint(void* ptr1, uint uint1, void* ptr2, void* ptr3, uint uint2,
            IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void CalliVoidPtr(void* ptr1, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtr(void* ptr1, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtr(void* ptr1, void* ptr2, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrPtrInt(void* ptr1, void* ptr2, void* ptr3, int int1, IntPtr functionPtr);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CalliIntPtrPtrPtrUint(void* ptr1, void* ptr2, void* ptr3, uint uint1,
            IntPtr functionPtr);

        /// <summary>
        /// Calls <see cref="IComparable{T}.CompareTo(T)"/> method on a generic <paramref name="left"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int CompareToConstrained<T>(ref T left, ref T right);

        /// <summary>
        /// Calls <see cref="IEquatable{T}.Equals(T)"/> method on a generic <paramref name="left"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IEquatable{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern bool EqualsConstrained<T>(ref T left, ref T right);

        /// <summary>
        /// Calls <see cref="object.GetHashCode"/> method on a generic <paramref name="obj"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int GetHashCodeConstrained<T>(ref T obj);

        /// <summary>
        /// Calls <see cref="IDisposable.Dispose"/> method on a generic <paramref name="obj"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IDisposable"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void DisposeConstrained<T>(ref T obj);

        /// <summary>
        /// Calls <see cref="IDelta{T}.AddDelta"/> method on a generic <paramref name="obj"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IDelta{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T AddDeltaConstrained<T>(ref T obj, ref T delta);

        /// <summary>
        /// Calls <see cref="IDelta{T}.GetDelta"/> method on a generic <paramref name="obj"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IDelta{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T GetDeltaConstrained<T>(ref T obj, ref T other);

        /// <summary>
        /// Calls <see cref="IInt64Diffable{T}.Add"/> method on a generic <paramref name="obj"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IInt64Diffable{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T AddLongConstrained<T>(ref T obj, long delta);

        /// <summary>
        /// Calls <see cref="IInt64Diffable{T}.Diff"/> method on a generic <paramref name="left"/> with the <seealso cref="OpCodes.Constrained"/> IL instruction.
        /// If the type <typeparamref name="T"/> does not implement <see cref="IInt64Diffable{T}"/> bad things will happen.
        /// Use static readonly bool field in a generic class that caches reflection check if the type implements the interface.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern long DiffLongConstrained<T>(ref T left, ref T right);

        /// <summary>
        /// Takes a (possibly null) object reference, plus an offset in bytes,
        /// adds them, and safely dereferences the target (untyped!) address in
        /// a way that the GC will be okay with.  It yields a value of type T.
        /// </summary>
        /// <param name="obj">An object (could be null)</param>
        /// <param name="offset">Byte offset from object pointer. If object is null this is just a native pointer (offset from zero pointer).
        /// It is not a pointer to an offset value but the offset itself. Originally it was <see cref="IntPtr"/> but that required casting.
        /// </param>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern T Get<T>(object obj, byte* offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetX<T>(object obj, IntPtr offset)
        {
            return GetAsObject<T>(obj, (byte*)offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetAsObject<T>(object obj, byte* offset)
        {
            return GetRef<T>(obj, offset);
        }

        [MethodImpl(MethodImplOptions.ForwardRef)]
        // ReSharper disable once UnusedTypeParameter
        public static extern IntPtr GetMethodPointer<T>();

        public static IntPtr GetMethodPointerForType(Type ty) // ...ForType suffix to simplify reflection, don't make it an overload, we are lazy
        {
            MethodInfo method = typeof(UnsafeEx).GetMethod("GetMethodPointer", BindingFlags.Static | BindingFlags.Public);
            // ReSharper disable once PossibleNullReferenceException
            MethodInfo generic = method.MakeGenericMethod(ty);
            return (IntPtr)generic.Invoke(null, null);
        }

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern object GetIndirect(object obj, byte* offset, IntPtr functionPtr);

        /// <summary>
        /// Takes a (possibly null) object reference, plus an offset in bytes,
        /// adds them, and safely dereferences the target (untyped!) address in
        /// a way that the GC will be okay with.  It yields a value of type T.
        /// </summary>
        /// <param name="obj">An object (could be null)</param>
        /// <param name="offset">Byte offset from object pointer. If object is null this is just a native pointer (offset from zero pointer).
        /// It is not a pointer to an offset value but the offset itself. Originally it was <see cref="IntPtr"/> but that required casting.
        /// </param>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern ref T GetRef<T>(object obj, byte* offset);

        /// <summary>
        /// Takes a (possibly null) object reference, plus an offset in bytes,
        /// adds them, and safely stores the value of type T in a way that the
        /// GC will be okay with.
        /// </summary>
        /// <param name="obj">An object (could be null).</param>
        /// <param name="offset">Byte offset from object pointer. If object is null this is just a native pointer (offset from zero pointer).
        /// It is not a pointer to an offset value but the offset itself. Originally it was <see cref="IntPtr"/> but that required casting.
        /// </param>
        /// <param name="val">A value to set.</param>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void Set<T>(object obj, byte* offset, T val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetX<T>(object obj, IntPtr offset, object val)
        {
            SetAsObject<T>(obj, (byte*)offset, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetAsObject<T>(object obj, byte* offset, object val)
        {
            GetRef<T>(obj, offset) = (dynamic)val;
        }

        /// <summary>
        /// Get a native method pointer to <see cref="Set{T}"/> method for type <typeparamref name="T"/>.
        /// The pointer should be used with <see cref="SetIndirect"/> method.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        // ReSharper disable once UnusedTypeParameter
        public static extern IntPtr SetMethodPointer<T>();

        /// <summary>
        /// Get a native method pointer to <see cref="Set{T}"/> method for type <paramref name="ty"/>.
        /// The pointer should be used with <see cref="SetIndirect"/> method.
        /// </summary>
        public static IntPtr SetMethodPointerForType(Type ty) // ...ForType suffix to simplify reflection, don't make it an overload, we are lazy
        {
            MethodInfo method = typeof(UnsafeEx).GetMethod("SetMethodPointer", BindingFlags.Static | BindingFlags.Public);
            // ReSharper disable once PossibleNullReferenceException
            MethodInfo generic = method.MakeGenericMethod(ty);
            return (IntPtr)generic.Invoke(null, null);
        }

        /// <summary>
        /// Set a value <paramref name="val"/> without generic parameters using <see cref="OpCodes.Calli"/> instruction for <see cref="Set{T}"/>
        /// method pointer obtained via <see cref="SetMethodPointer{T}"/> or <see cref="SetMethodPointerForType"/> methods.
        /// </summary>
        /// <remarks>Value <paramref name="val"/> is cast to underlying type as `(T)(dynamic)val`.</remarks>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void SetIndirect(object obj, byte* offset, object val, IntPtr functionPtr);

        /// <summary>
        /// Computes the number of bytes offset from an array object reference
        /// to its first element, in a way the GC will be okay with.
        /// </summary>
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int ElemOffset<T>(T[] arr);

        internal static int ElemOffset<T>()
        {
            return ElemOffset<T>(new T[1]);
        }

        public static int ElemOffsetOfType(Type ty)
        {
            MethodInfo method = typeof(UnsafeEx).GetMethod("ElemOffset", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo genericMethod = method.MakeGenericMethod(ty);
            return (int)genericMethod.Invoke(null, null);
        }

        internal static int ElemSize<T>()
        {
            return Unsafe.SizeOf<T>();
        }

        public static int ElemSizeOfType(Type ty)
        {
            MethodInfo method = typeof(UnsafeEx).GetMethod("ElemSize", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo genericMethod = method.MakeGenericMethod(ty);
            return (int)genericMethod.Invoke(null, null);
        }
    }
}
