// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace Spreads
{
    public interface IDelta<T>
    {
        T AddDelta(T delta);

        T GetDelta(T other);
    }

    public interface IInt64Diffable<T> : IComparable<T>
    {
        // Token: 0x0600001D RID: 29
        T Add(long diff);

        // Token: 0x0600001E RID: 30
        long Diff(T other);
    }
}
