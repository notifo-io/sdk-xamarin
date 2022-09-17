// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections;
using System.Collections.Generic;

namespace Notifo.SDK.Helpers
{
    internal class SlidingSet<T> : ICollection<T>
    {
        private readonly HashSet<T> itemSet = new HashSet<T>();
        private readonly LinkedList<T> itemHistory = new LinkedList<T>();

        public int Count => itemSet.Count;

        public bool IsReadOnly => false;

        public void Clear()
        {
            itemSet.Clear();
            itemHistory.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            itemHistory.CopyTo(array, arrayIndex);
        }

        public bool Contains(T item)
        {
            return itemSet.Contains(item);
        }

        public void Add(T item)
        {
            Add(item, 0);
        }

        public void Add(T item, int capacity)
        {
            if (!itemSet.Add(item))
            {
                itemHistory.Remove(item);
                itemHistory.AddLast(item);
                return;
            }

            if (capacity > 0 && itemSet.Count > capacity)
            {
                RemoveFirst();
            }

            itemHistory.AddLast(item);
        }

        private void RemoveFirst()
        {
            var node = itemHistory.First;

            if (node != null)
            {
                itemSet.Remove(node.Value);
                itemHistory.RemoveFirst();
            }
        }

        public bool Remove(T item)
        {
            var hasRemoved = itemSet.Remove(item);

            if (hasRemoved)
            {
                itemHistory.Remove(item);
            }

            return hasRemoved;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return itemHistory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return itemHistory.GetEnumerator();
        }
    }
}
