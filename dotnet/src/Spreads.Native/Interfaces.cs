// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace Spreads
{
    /// <summary>
    /// An interface for a type whose value could be stored as a delta from a previous value and recovered later.
    /// Delta is the same type as a value. E.g. all numeric types could implement this interface with plus and minus operations.
    /// </summary>
    public interface IDelta<T>
    {
        /// <summary>
        /// This + Delta = New valid value
        /// </summary>
        T AddDelta(T delta);

        /// <summary>
        /// This - Other = Delta so that: other.AddDelta(this.GetDelta(other)) == this.
        /// </summary>
        T GetDelta(T other);
    }

    /// <summary>
    /// An interface for a type whose value could be stored as an Int64 delta from a previous value and recovered later by addition.
    /// </summary>
    /// <seealso cref="IDelta{T}"/>
    public interface IInt64Diffable<T> : IComparable<T>
    {
        T Add(long diff);

        long Diff(T other);
    }
}
