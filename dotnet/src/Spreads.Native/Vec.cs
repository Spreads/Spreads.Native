// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spreads.Native
{
    /// <summary>
    /// Typed native or managed vector.
    /// </summary>
    /// <remarks>Not thread safe and not safe at all</remarks>
    public readonly unsafe struct Vec<T>
    {
        internal readonly T[] _array;
        internal readonly byte* _offset;
        /// <summary>
        ///
        /// </summary>
        internal readonly int _length;
        internal readonly int _runtimeTypeId;

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the entirety of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(T[] array)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }
            _array = array;
            _offset = (byte*)VecTypeHelper<T>.RuntimeVecInfo.ElemOffset;
            _length = array.Length;
            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the target unmanaged buffer. This
        /// is quite dangerous, because we are creating arbitrarily typed T's
        /// out of a void*-typed block of memory.  And the length is not checked.
        /// But if this creation is correct, then all subsequent uses are correct.
        /// </summary>
        /// <param name="ptr">An unmanaged pointer to memory.</param>
        /// <param name="length">The number of T elements the memory contains.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(void* ptr, int length)
        {
            if (length < 0)
            {
                VecThrowHelper.ThrowNegativeLength();
            }

            if (ptr == null && length != 0)
            {
                VecThrowHelper.ThrowNullPointer();
            }

            _array = null;
            _offset = (byte*)ptr;
            _length = length;
            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        /// <summary>
        /// An internal helper for creating Vecs.  Not for public use.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Vec(T[] array, byte* offset, int length, int runtimeTypeId)
        {
            Debug.Assert(runtimeTypeId == VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId);
            _array = array;
            _offset = offset;
            _length = length;
            _runtimeTypeId = runtimeTypeId;
        }

        /// <summary>
        /// Returns untyped <see cref="Vec"/> with the same data as this <see cref="Vec{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec AsVec()
        {
            return new Vec(_array, _offset, _length, _runtimeTypeId);
        }

        /// <summary>
        /// Get the total number of elements in Vec.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        /// <summary>
        /// Fetches the element at the specified index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified index is not in range (&lt;0 or &gt;&eq;length).
        /// </exception>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRange();
                }

                // TODO check if Unsafe.SizeOf is faster here
                return ref UnsafeEx.GetRef<T>(_array, _offset + index * VecTypeHelper<T>.RuntimeVecInfo.ElemSize);
            }
        }
    }

    /// <summary>
    /// Untyped native or managed vector.
    /// </summary>
    /// <remarks>Not thread safe and not safe at all</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 24)]
    public readonly unsafe struct Vec
    {
        internal readonly Array _array;
        internal readonly byte* _offset;
        /// <summary>
        ///
        /// </summary>
        internal readonly int _length;
        internal readonly int _runtimeTypeId;

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the entirety of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(Array array)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }

            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            _array = array;
            _offset = (byte*)vti.ElemOffset;
            _length = array.Length;
            _runtimeTypeId = vti.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the target unmanaged buffer. This
        /// is quite dangerous, because we are creating arbitrarily typed T's
        /// out of a void*-typed block of memory.  And the length is not checked.
        /// But if this creation is correct, then all subsequent uses are correct.
        /// </summary>
        /// <param name="ptr">An unmanaged pointer to memory.</param>
        /// <param name="length">The number of T elements the memory contains.</param>
        /// <param name="elementType">Element type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(void* ptr, int length, Type elementType)
        {
            if (length < 0)
            {
                VecThrowHelper.ThrowNegativeLength();
            }

            if (ptr == null && length != 0)
            {
                VecThrowHelper.ThrowNullPointer();
            }

            ref var vti = ref VecTypeHelper.GetInfo(elementType);

            _array = null;
            _offset = (byte*)ptr;
            _length = length;
            _runtimeTypeId = vti.RuntimeTypeId;
        }

        /// <summary>
        /// An internal helper for creating Vecs.  Not for public use.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Vec(Array array, byte* offset, int length, int runtimeTypeId)
        {
#if DEBUG
            Debug.Assert(runtimeTypeId == VecTypeHelper.GetInfo(array.GetType().GetElementType()).RuntimeTypeId);
#endif
            _array = array;
            _offset = offset;
            _length = length;
            _runtimeTypeId = runtimeTypeId;
        }

        /// <summary>
        /// Returns typed <see cref="Vec{T}"/> with the same data as this <see cref="Vec"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec<T> As<T>()
        {
            var vtidx = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
            if (vtidx != _runtimeTypeId)
            {
                VecThrowHelper.ThrowWrongAsType();
            }

            return new Vec<T>(_array as T[], _offset, _length, _runtimeTypeId);
        }

        /// <summary>
        /// Get the total number of elements in Vec.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        /// <summary>
        /// Fetches the element at the specified index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified index is not in range (&lt;0 or &gt;&eq;length).
        /// </exception>
        public object this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRange();
                }

                ref var vti = ref VecTypeHelper.GetInfo(_runtimeTypeId);
                return UnsafeEx.GetIndirect(_array, _offset + index * vti.ElemSize, vti.UnsafeGetterPtr);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRange();
                }

                ref var vti = ref VecTypeHelper.GetInfo(_runtimeTypeId);
                UnsafeEx.SetIndirect(_array, _offset + index * vti.ElemSize, value, vti.UnsafeSetterPtr);
            }
        }
    }

    internal static class VecThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArrayIsNull()
        {
            throw new ArgumentNullException("array");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNegativeLength()
        {
            throw new ArgumentOutOfRangeException("length");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNullPointer()
        {
            throw new ArgumentOutOfRangeException("ptr");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowIndexOutOfRange()
        {
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowWrongAsType()
        {
            throw new InvalidOperationException("Wrong type in Vec to Vec<T> conversion");
        }
    }
}
