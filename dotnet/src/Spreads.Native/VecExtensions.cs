// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace Spreads.Native
{
    /// <summary>
    /// Extension methods for <see cref="Vec{T}"/>.
    /// </summary>
    public static class VecExtensions
    {
        /// <summary>
        /// Creates a new Vec over the portion of the target array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec<T> AsVec<T>(this T[] array, int start) => Vec<T>.Create(array, start);

        /// <summary>
        /// Creates a new Vec over the portion of the target array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec AsVec(this Array array, int start) => Vec.Create(array, start);

        /// <summary>
        /// Move a block of values inside vector. Source and destination could overlap.
        /// </summary>
        public static void MoveBlock<T>(this Vec vec, int start, int length, int destination)
        {
            if ((uint)start > (uint)vec._length || (uint)length > (uint)(vec._length - start))
            { VecThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start); }
            // TODO MemoryMarshal.CreateReadOnlySpan and manual bound ckecks
            var span = vec.AsSpan<T>();
            span.Slice(start, length).CopyTo(span.Slice(destination, length));
        }

        /// <summary>
        /// Move a block of values inside vector. Source and destination could overlap.
        /// </summary>
        public static void MoveBlock<T>(this Vec<T> vec, int start, int length, int destination)
        {
            var span = vec.Span;
            span.Slice(start, length).CopyTo(span.Slice(destination, length));
        }
    }
}