// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spreads.Native
{
    // Vec<T> and Vec implementations must be identical.

    /// <summary>
    /// Typed native or managed vector.
    /// </summary>
    /// <remarks>Not thread safe and not safe at all</remarks>
    public readonly unsafe struct Vec<T> : IEnumerable<T>
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
            // ReSharper disable once PossibleNullReferenceException
            _length = array.Length;
            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the portion of the target array beginning
        /// at 'start' index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified start index is not in range.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(T[] array, int start)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }
            // ReSharper disable once PossibleNullReferenceException
            var arrLen = array.Length;
            if (unchecked((uint)start) >= unchecked((uint)arrLen))
            {
                VecThrowHelper.ThrowStartOrLengthOutOfRange();
            }

            if (start < arrLen)
            {
                _array = array;
                _offset = (byte*)(VecTypeHelper<T>.RuntimeVecInfo.ElemOffset + start * Unsafe.SizeOf<T>());
                _length = array.Length - start;
                _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
            }
            else
            {
                _array = null;
                _offset = null;
                _length = 0;
                _runtimeTypeId = 0;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the portion of the target array beginning
        /// at 'start' index and ending at 'end' index (exclusive).
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <param name="length">The length of the Vec.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified start or end index is not in range.
        /// </exception>
        public Vec(T[] array, int start, int length)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }
            // ReSharper disable once PossibleNullReferenceException
            var arrLen = array.Length;
            if (unchecked((uint)start) + unchecked((uint)length) >= unchecked((uint)arrLen))
            {
                VecThrowHelper.ThrowStartOrLengthOutOfRange();
            }
            if (start < arrLen)
            {
                _array = array;
                _offset = (byte*)(VecTypeHelper<T>.RuntimeVecInfo.ElemOffset + start * Unsafe.SizeOf<T>());
                _length = length;
                _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
            }
            else
            {
                _array = null;
                _offset = null;
                _length = 0;
                _runtimeTypeId = 0;
            }
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

        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_array == null)
                {
                    return new Span<T>(_offset, _length);
                }

                return new Span<T>(_array, (int)(checked((uint)_offset - VecTypeHelper<T>.RuntimeVecInfo.ElemOffset)) / Unsafe.SizeOf<T>(), _length);
            }
        }

        /// <summary>
        /// Fetches the element at the specified index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the specified index is not in range.
        /// </exception>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRange();
                }

                return GetUnchecked(index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRange();
                }

                UnsafeEx.Set<T>(_array, _offset + index * Unsafe.SizeOf<T>(), value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetUnchecked(int index)
        {
            return UnsafeEx.Get<T>(_array, _offset + index * Unsafe.SizeOf<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRef(int index)
        {
            if (unchecked((uint)index) >= unchecked((uint)_length))
            {
                VecThrowHelper.ThrowIndexOutOfRange();
            }
            return ref GetRefUnchecked(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetRefUnchecked(int index)
        {
            return ref UnsafeEx.GetRef<T>(_array, _offset + index * Unsafe.SizeOf<T>());
        }

        /// <summary>
        /// Checks to see if two Vecs point at the same memory.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReferenceEquals(Vec<T> other)
        {
            return _array == other._array
                   && _offset == other._offset
                   && _length == other._length
                   && _runtimeTypeId == other._runtimeTypeId;
        }

        /// <summary>
        /// Returns an enumerator over the Slice's entire contents.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// A struct-based enumerator, to make fast enumerations possible.
        /// This isn't designed for direct use, instead see GetEnumerator.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private int _position;  // The current position.
            private Vec<T> _vec;    // The slice being enumerated.

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(Vec<T> vec)
            {
                _vec = vec;
                _position = -1;
            }

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _vec.GetUnchecked(_position);
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _vec = default;
                _position = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                _position++;
                return _vec._length > _position;
            }

            public void Reset()
            {
                _position = -1;
            }
        }
    }

    /// <summary>
    /// Untyped native or managed vector.
    /// </summary>
    /// <remarks>Not thread safe and not safe at all</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 24)]
    public readonly unsafe struct Vec : IEnumerable
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

            // ReSharper disable once PossibleNullReferenceException
            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            _array = array;
            _offset = (byte*)vti.ElemOffset;
            _length = array.Length;
            _runtimeTypeId = vti.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new <see cref="Vec"/> over the portion of the target array beginning
        /// at 'start' index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified start index is not in range.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(Array array, int start)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }
            // ReSharper disable once PossibleNullReferenceException
            var arrLen = array.Length;
            if (unchecked((uint)start) >= unchecked((uint)arrLen))
            {
                VecThrowHelper.ThrowStartOrLengthOutOfRange();
            }

            // ReSharper disable once PossibleNullReferenceException
            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            if (start < arrLen)
            {
                _array = array;
                _offset = (byte*)(vti.ElemOffset + start * vti.ElemSize);
                _length = array.Length - start;
                _runtimeTypeId = vti.RuntimeTypeId;
            }
            else
            {
                _array = null;
                _offset = null;
                _length = 0;
                _runtimeTypeId = 0;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Vec{T}"/> over the portion of the target array beginning
        /// at 'start' index and ending at 'end' index (exclusive).
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <param name="length">The length of the Vec.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified start or end index is not in range.
        /// </exception>
        public Vec(Array array, int start, int length)
        {
            if (array == null)
            {
                VecThrowHelper.ThrowArrayIsNull();
            }
            // ReSharper disable once PossibleNullReferenceException
            var arrLen = array.Length;
            if (unchecked((uint)start) + unchecked((uint)length) >= unchecked((uint)arrLen))
            {
                VecThrowHelper.ThrowStartOrLengthOutOfRange();
            }

            // ReSharper disable once PossibleNullReferenceException
            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            if (start < arrLen)
            {
                _array = array;
                _offset = (byte*)(vti.ElemOffset + start * vti.ElemSize);
                _length = length;
                _runtimeTypeId = vti.RuntimeTypeId;
            }
            else
            {
                _array = null;
                _offset = null;
                _length = 0;
                _runtimeTypeId = 0;
            }
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

            if (elementType == null)
            {
                VecThrowHelper.ThrowTypeIsNull();
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
        /// Thrown when the specified index is not in range.
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

                return GetUnchecked(index);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object GetUnchecked(int index)
        {
            ref var vti = ref VecTypeHelper.GetInfo(_runtimeTypeId);
            return UnsafeEx.GetIndirect(_array, _offset + index * vti.ElemSize, vti.UnsafeGetterPtr);
        }

        /// <summary>
        /// Checks to see if two Vecs point at the same memory.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        public bool ReferenceEquals(Vec other)
        {
            return _array == other._array
                   && _offset == other._offset
                   && _length == other._length
                   && _runtimeTypeId == other._runtimeTypeId;
        }

        /// <summary>
        /// Returns an enumerator over the Slice's entire contents.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// A struct-based enumerator, to make fast enumerations possible.
        /// This isn't designed for direct use, instead see GetEnumerator.
        /// </summary>
        public struct Enumerator : IEnumerator
        {
            private Vec _vec;    // The slice being enumerated.
            private int _position; // The current position.

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(Vec vec)
            {
                _vec = vec;
                _position = -1;
            }

            public object Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _vec[_position];
            }

            object IEnumerator.Current => Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                _vec = default;
                _position = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                return ++_position < _vec.Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _position = -1;
            }
        }
    }

    internal static class VecThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArrayIsNull()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentNullException("array");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowTypeIsNull()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentNullException("type");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNegativeLength()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("length");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNullPointer()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("ptr");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowIndexOutOfRange()
        {
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowStartOrLengthOutOfRange()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("start or length");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowWrongAsType()
        {
            throw new InvalidOperationException("Wrong type in Vec to Vec<T> conversion");
        }
    }
}
