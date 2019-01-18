// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace Spreads.Native
{
    // TODO move all to the other part and remove duplicates

    internal static partial class VecThrowHelper
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
        internal static void ThrowStartOrLengthOutOfRange()
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("start or length");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowWrongCastType<T>()
        {
            throw new InvalidOperationException("Wrong type in object to T conversion: T is " + typeof(T).Name);
        }
    }
}