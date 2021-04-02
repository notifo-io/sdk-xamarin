// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Linq;
using Newtonsoft.Json;
using Notifo.SDK.Helpers;
using Xunit;

namespace Notifo.SDK.UnitTests
{
    public class SlidingSetTests
    {
        [Fact]
        public void ShouldBeSerializable()
        {
            var set = new SlidingSet<int>(10);

            Enumerable.Range(1, 20)
                .ToList()
                .ForEach(x => set.Add(x));

            var serialized = JsonConvert.SerializeObject(set);
            var deserialized = JsonConvert.DeserializeObject<SlidingSet<int>>(serialized);

            deserialized.Add(21);

            Assert.True(deserialized.Contains(12));
            Assert.True(deserialized.Contains(21));
            Assert.False(deserialized.Contains(11));
        }

        [Theory]
        [InlineData(100, 100)]
        [InlineData(2000, 1000)]
        public void Add_ShouldRespectCapacity(int capacity, int expected)
        {
            var set = new SlidingSet<int>(capacity);

            Enumerable.Range(1, 1000)
                .ToList()
                .ForEach(x => set.Add(x));

            Assert.Equal(expected, set.Count);
        }

        [Fact]
        public void Add_ShouldRemoveOldestItem_IfExceedsCapacity()
        {
            var set = new SlidingSet<int>(10);

            Enumerable.Range(1, 20)
                .ToList()
                .ForEach(x => set.Add(x));

            Assert.True(set.Contains(11));
            Assert.True(set.Contains(20));
            Assert.False(set.Contains(1));
            Assert.False(set.Contains(9));
        }
    }
}
