// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable IdentifierTypo
#pragma warning disable 1584

namespace Spreads
{
    /// <summary>
    /// Contains unsafe IL methods useful for Spreads.
    /// </summary>
    /// <seealso cref="System.Runtime.CompilerServices.Unsafe"/>
    public static unsafe class UnsafeEx
    {
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern void CalliDataBlock<TKey, TValue>(object dlg, object block, int index, ref TKey key, ref TValue value, IntPtr fPtr);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(ref T source)
        {
            return Unsafe.ReadUnaligned<T>(ref Unsafe.As<T, byte>(ref source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(ref T destination, T value)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Ceq(int first, int second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Cgt(int first, int second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Clt(int first, int second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Ceq(long first, long second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Cgt(long first, long second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Clt(long first, long second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Ceq(nint first, nint second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Cgt(nint first, nint second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Clt(nint first, nint second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Ceq(float first, float second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Ceq(double first, double second);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int BoolAsInt(bool boolValue);
    }
}
