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
    // TODO replace Span name
    // Vec<T> and Vec implementations must be identical.

    /// <summary>
    /// Typed native or managed vector.
    /// </summary>
    /// <remarks>Not thread safe and not safe at all</remarks>
    public readonly unsafe struct Vec<T> : IEnumerable<T>
    {
        internal readonly Pinnable<T> _pinnable;
        internal readonly IntPtr _byteOffset;
        internal readonly int _length;

        internal readonly int _runtimeTypeId;

        /// <summary>
        /// Creates a new Vec over the entirety of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(T[] array)
        {
            if (array == null)
            {
                this = default;
                return; // returns default
            }

            if (default(T) == null && array.GetType() != typeof(T[]))
            {
                VecThrowHelper.ThrowArrayTypeMismatchException();
            }

            _length = array.Length;
            _pinnable = Unsafe.As<Pinnable<T>>(array);
            _byteOffset = VecHelpers.PerTypeValues<T>.ArrayAdjustment;

            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        // This is a constructor that takes an array and start but not length. The reason we expose it as a static method as a constructor
        // is to mirror the actual api shape. This overload of the constructor was removed from the api surface area due to possible
        // confusion with other overloads that take an int parameter that don't represent a start index.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vec<T> Create(T[] array, int start)
        {
            if (array == null)
            {
                if (start != 0)
                    VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }
            if (default(T) == null && array.GetType() != typeof(T[]))
                VecThrowHelper.ThrowArrayTypeMismatchException();
            if (unchecked((uint)start) > unchecked((uint)array.Length))
                VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            IntPtr byteOffset = VecHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
            int length = array.Length - start;
            return new Vec<T>(pinnable: Unsafe.As<Pinnable<T>>(array), byteOffset: byteOffset, length: length, runtimeTypeId: VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId);
        }

        /// <summary>
        /// Creates a new Vec over the portion of the target array beginning
        /// at 'start' index and ending at 'end' index (exclusive).
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the Vec.</param>
        /// <param name="length">The number of items in the Vec.</param>
        /// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
        /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(T[] array, int start, int length)
        {
            if (array == null)
            {
                if (start != 0 || length != 0)
                    VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                this = default;
                return; // returns default
            }
            if (default(T) == null && array.GetType() != typeof(T[]))
            { VecThrowHelper.ThrowArrayTypeMismatchException(); }
            if (unchecked((uint)start) > unchecked((uint)array.Length) || unchecked((uint)length) > unchecked((uint)(array.Length - start)))
            { VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start); }

            _length = length;
            _pinnable = Unsafe.As<Pinnable<T>>(array);
            _byteOffset = VecHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new Vec over the target unmanaged buffer.  Clearly this
        /// is quite dangerous, because we are creating arbitrarily typed T's
        /// out of a void*-typed block of memory.  And the length is not checked.
        /// But if this creation is correct, then all subsequent uses are correct.
        /// </summary>
        /// <param name="pointer">An unmanaged pointer to memory.</param>
        /// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="T"/> is reference type or contains pointers and hence cannot be stored in unmanaged memory.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is negative.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(void* pointer, int length)
        {
            if (VecHelpers.IsReferenceOrContainsReferences<T>())
            { VecThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T)); }
            if (length < 0)
            { VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start); }

            _length = length;
            _pinnable = null;
            _byteOffset = new IntPtr(pointer);
            _runtimeTypeId = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
        }

        // Constructor for internal use only.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Vec(Pinnable<T> pinnable, IntPtr byteOffset, int length, int runtimeTypeId)
        {
            Debug.Assert(length >= 0);

            _length = length;
            _pinnable = pinnable;
            _byteOffset = byteOffset;
            _runtimeTypeId = runtimeTypeId;
        }

        /// <summary>
        /// Returns untyped <see cref="Vec"/> with the same data as this <see cref="Vec{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec AsVec()
        {
            return new Vec(Unsafe.As<Array>(_pinnable), _byteOffset, _length, _runtimeTypeId);
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
                if (_pinnable == null)
                {
                    return new Span<T>((void*)_byteOffset, _length);
                }

                return new Span<T>(Unsafe.As<T[]>(_pinnable), (int)(checked((uint)_byteOffset - VecTypeHelper<T>.RuntimeVecInfo.ArrayOffsetAdjustment)) / Unsafe.SizeOf<T>(), _length);
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
                    VecThrowHelper.ThrowIndexOutOfRangeException();
                }

                return GetUnchecked(index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRangeException();
                }

                GetRefUnchecked(index) = value;
            }
        }

        /// <summary>
        /// Fetches the element at the specified index without bound checks.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetUnchecked(int index)
        {
            // TODO known types
            if (typeof(T) == typeof(bool)
                || typeof(T) == typeof(byte) 
                || typeof(T) == typeof(sbyte) 
                || typeof(T) == typeof(short) 
                || typeof(T) == typeof(ushort) 
                || typeof(T) == typeof(int) 
                || typeof(T) == typeof(uint) 
                || typeof(T) == typeof(long) 
                || typeof(T) == typeof(ulong)
                || typeof(T) == typeof(char)
                || typeof(T) == typeof(float)
                || typeof(T) == typeof(double)
                || typeof(T) == typeof(decimal)
            )
            {
                return UnsafeEx.DangerousGetAtIndex<T>(_pinnable, _byteOffset, index);
            }
            return GetRefUnchecked(index);
        }

        /// <summary>
        /// Returns a reference to a value at index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRef(int index)
        {
            if (unchecked((uint)index) >= unchecked((uint)_length))
            {
                VecThrowHelper.ThrowIndexOutOfRangeException();
            }
            return ref GetRefUnchecked(index);
        }

        /// <summary>
        /// Returns a reference to a value at index without bound checks.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRefUnchecked(int index)
        {
            if (_pinnable == null)
            {
                return ref Unsafe.Add<T>(ref Unsafe.AsRef<T>(_byteOffset.ToPointer()), index);
            }
            else
            {
                return ref Unsafe.Add<T>(ref Unsafe.AddByteOffset<T>(ref _pinnable.Data, _byteOffset), index);
            }
        }

        /// <summary>
        /// Checks to see if two Vecs point at the same memory.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReferenceEquals(Vec<T> other)
        {
            return _pinnable == other._pinnable
                   && _byteOffset == other._byteOffset
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
        internal readonly Array _pinnable;
        internal readonly IntPtr _byteOffset;
        internal readonly int _length;
        internal readonly int _runtimeTypeId;

        /// <summary>
        /// Creates a new Vec over the entirety of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(Array array)
        {
            if (array == null)
            {
                this = default;
                return; // returns default
            }

            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            _length = array.Length;
            _pinnable = array;
            _byteOffset = (IntPtr)vti.ArrayOffsetAdjustment;

            _runtimeTypeId = vti.RuntimeTypeId;
        }

        // This is a constructor that takes an array and start but not length. The reason we expose it as a static method as a constructor
        // is to mirror the actual api shape. This overload of the constructor was removed from the api surface area due to possible
        // confusion with other overloads that take an int parameter that don't represent a start index.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vec Create(Array array, int start)
        {
            if (array == null)
            {
                if (start != 0)
                    VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }

            if (unchecked((uint)start) > unchecked((uint)array.Length))
                VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            IntPtr byteOffset = (IntPtr)(vti.ArrayOffsetAdjustment + start * vti.ElemSize);
            int length = array.Length - start;
            return new Vec(array: array, byteOffset: byteOffset, length: length, runtimeTypeId: vti.RuntimeTypeId);
        }

        /// <summary>
        /// Creates a new Vec over the portion of the target array beginning
        /// at 'start' index and ending at 'end' index (exclusive).
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the Vec.</param>
        /// <param name="length">The number of items in the Vec.</param>
        /// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
        /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;=Length).
        /// </exception>
        public Vec(Array array, int start, int length)
        {
            if (array == null)
            {
                if (start != 0 || length != 0)
                    VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                this = default;
                return; // returns default
            }

            // ReSharper disable once PossibleNullReferenceException
            ref var vti = ref VecTypeHelper.GetInfo(array.GetType().GetElementType());

            if (unchecked((uint)start) > unchecked((uint)array.Length) || unchecked((uint)length) > unchecked((uint)(array.Length - start)))
            { VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start); }

            _length = length;
            _pinnable = array;
            _byteOffset = (IntPtr)(vti.ArrayOffsetAdjustment + start * vti.ElemSize);
            _runtimeTypeId = vti.RuntimeTypeId;
        }

        /// <summary>
        /// Creates a new Vec over the target unmanaged buffer.  Clearly this
        /// is quite dangerous, because we are creating arbitrarily typed T's
        /// out of a void*-typed block of memory.  And the length is not checked.
        /// But if this creation is correct, then all subsequent uses are correct.
        /// </summary>
        /// <param name="pointer">An unmanaged pointer to memory.</param>
        /// <param name="length">The number of elements the memory contains.</param>
        /// <param name="elementType">Element type.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is negative.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec(void* pointer, int length, Type elementType)
        {
            ref var vti = ref VecTypeHelper.GetInfo(elementType);

            if (vti.IsReferenceOrContainsReferences)
            { VecThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(elementType); }
            if (length < 0)
            { VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start); }

            if (pointer == null && length != 0)
            {
                VecThrowHelper.ThrowNullPointer();
            }

            if (elementType == null)
            {
                VecThrowHelper.ThrowTypeIsNull();
            }

            _pinnable = null;
            _byteOffset = (IntPtr)pointer;
            _length = length;
            _runtimeTypeId = vti.RuntimeTypeId;
        }

        /// <summary>
        /// An internal helper for creating Vecs.  Not for public use.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Vec(Array array, IntPtr byteOffset, int length, int runtimeTypeId)
        {
#if DEBUG
            Debug.Assert(runtimeTypeId == VecTypeHelper.GetInfo(array.GetType().GetElementType()).RuntimeTypeId);
#endif
            _pinnable = array;
            _byteOffset = byteOffset;
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
                VecThrowHelper.ThrowWrongCastType<T>();
            }

            return new Vec<T>(Unsafe.As<Pinnable<T>>(_pinnable), _byteOffset, _length, _runtimeTypeId);
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
                    VecThrowHelper.ThrowIndexOutOfRangeException();
                }

                return GetUnchecked(index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)index) >= unchecked((uint)_length))
                {
                    VecThrowHelper.ThrowIndexOutOfRangeException();
                }

                ref var vti = ref VecTypeHelper.GetInfo(_runtimeTypeId);
                UnsafeEx.SetIndirect(_pinnable, _byteOffset, index, value, vti.UnsafeSetterPtr);
            }
        }

        /// <summary>
        /// Fetches the element at the specified index without bound checks.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetUnchecked(int index)
        {
            ref var vti = ref VecTypeHelper.GetInfo(_runtimeTypeId);
            return UnsafeEx.GetIndirect(_pinnable, _byteOffset, index, vti.UnsafeGetterPtr);
        }

        /// <summary>
        /// Get a typed value at index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>(int index)
        {
            if (unchecked((uint)index) >= unchecked((uint)_length))
            {
                VecThrowHelper.ThrowIndexOutOfRangeException();
            }

            var vtidx = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
            if (vtidx != _runtimeTypeId)
            {
                VecThrowHelper.ThrowWrongCastType<T>();
            }

            return GetRefUnchecked<T>(index);
        }

        /// <summary>
        /// Get a typed value at index without type or bounds check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetUnchecked<T>(int index)
        {
            return GetRefUnchecked<T>(index);
        }

        /// <summary>
        /// Get a typed reference to a value at index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRef<T>(int index)
        {
            var vtidx = VecTypeHelper<T>.RuntimeVecInfo.RuntimeTypeId;
            if (vtidx != _runtimeTypeId)
            {
                VecThrowHelper.ThrowWrongCastType<T>();
            }

            return ref GetRefUnchecked<T>(index);
        }

        /// <summary>
        /// Get a typed reference to a value at index without type or bounds check.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRefUnchecked<T>(int index)
        {
            return ref UnsafeEx.GetRef<T>(_pinnable, _byteOffset, index);
        }

        /// <summary>
        /// Checks to see if two Vecs point at the same memory.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        public bool ReferenceEquals(Vec other)
        {
            return _pinnable == other._pinnable
                   && _byteOffset == other._byteOffset
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
                get => _vec.GetUnchecked(_position);
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
}
