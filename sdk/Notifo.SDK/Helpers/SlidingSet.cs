// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Notifo.SDK.Helpers
{
    internal class SlidingSet<T>
    {
        [JsonProperty]
        private readonly HashSet<T> set;

        [JsonProperty]
        private readonly LinkedList<T> history;

        [JsonProperty]
        private readonly int capacity;

        [JsonIgnore]
        public int Count => set.Count;

        public SlidingSet(int capacity)
        {
            this.capacity = capacity;

            set = new HashSet<T>();
            history = new LinkedList<T>();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void Add(T item)
        {
            if (set.Contains(item))
            {
                history.Remove(item);
                history.AddLast(item);

                return;
            }

            if (set.Count >= capacity)
            {
                RemoveFirst();
            }

            set.Add(item);
            history.AddLast(item);
        }

        private void RemoveFirst()
        {
            var node = history.First;
            if (node != null)
            {
                set.Remove(node.Value);
                history.RemoveFirst();
            }
        }
    }
}
