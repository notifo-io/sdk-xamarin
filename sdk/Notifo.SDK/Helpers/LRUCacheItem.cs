// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable SA1401 // Fields must be private

namespace Notifo.SDK.Helpers;

internal sealed class LRUCacheItem<TKey, TValue>
{
    public TKey Key;

    public TValue Value;

    public int Size;
}
