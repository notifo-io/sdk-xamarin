// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Notifo.SDK.Helpers
{
    internal sealed class LRUCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>> cacheMap = new Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>>();
        private readonly LinkedList<LRUCacheItem<TKey, TValue>> cacheHistory = new LinkedList<LRUCacheItem<TKey, TValue>>();
        private int totalCapacity;
        private int totalSize;

        public int Size => totalSize;

        public LRUCache(int capacity)
        {
            EnsureCapacity(capacity);
        }

        public void EnsureCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be positive.", nameof(capacity));
            }

            lock (cacheMap)
            {
                while (totalCapacity > capacity)
                {
                    RemoveFirst();
                }

                totalCapacity = capacity;
            }
        }

        public void Clear()
        {
            lock (cacheMap)
            {
                cacheHistory.Clear();
                cacheMap.Clear();

                totalSize = 0;
            }
        }

        public bool Set(TKey key, TValue value, int size = 1)
        {
            lock (cacheMap)
            {
                if (cacheMap.TryGetValue(key, out var node))
                {
                    node.Value.Value = value;

                    cacheHistory.Remove(node);
                    cacheHistory.AddLast(node);

                    cacheMap[key] = node;

                    return true;
                }

                totalSize += size;

                if (totalSize > totalCapacity)
                {
                    RemoveFirst();
                }

                var cacheItem = new LRUCacheItem<TKey, TValue> { Key = key, Value = value, Size = size };

                node = new LinkedListNode<LRUCacheItem<TKey, TValue>>(cacheItem);

                cacheMap.Add(key, node);
                cacheHistory.AddLast(node);

                return false;
            }
        }

        public bool Remove(TKey key)
        {
            lock (cacheMap)
            {
                if (cacheMap.TryGetValue(key, out var node))
                {
                    cacheMap.Remove(key);
                    cacheHistory.Remove(node);

                    return true;
                }

                return false;
            }
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            lock (cacheMap)
            {
                value = default!;

                if (cacheMap.TryGetValue(key, out var node))
                {
                    value = node.Value.Value;

                    cacheHistory.Remove(node);
                    cacheHistory.AddLast(node);

                    return true;
                }

                return false;
            }
        }

        public bool Contains(TKey key)
        {
            lock (cacheMap)
            {
                return cacheMap.ContainsKey(key);
            }
        }

        private void RemoveFirst()
        {
            var node = cacheHistory.First;

            if (node != null)
            {
                cacheMap.Remove(node.Value.Key);
                cacheHistory.RemoveFirst();

                totalSize -= node.Value.Size;
            }
        }
    }
}