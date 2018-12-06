// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace Spreads
{
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

    public interface IInt64Diffable<T> : IComparable<T>
    {
        T Add(long diff);

        long Diff(T other);
    }
}
