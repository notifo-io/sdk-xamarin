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

namespace Notifo.SDK.UnitTests;

public class SlidingSetTests
{
    [Fact]
    public void ShouldBeSerializable()
    {
        var set = new SlidingSet<int>();

        Enumerable.Range(1, 20)
            .ToList()
            .ForEach(x => set.Add(x, 10));

        var serialized = JsonConvert.SerializeObject(set);
        var deserialized = JsonConvert.DeserializeObject<SlidingSet<int>>(serialized);

        deserialized.Add(21, 10);

        Assert.DoesNotContain(11, deserialized);
        Assert.Contains(12, deserialized);
        Assert.Contains(21, deserialized);
    }

    [Theory]
    [InlineData(100, 100)]
    [InlineData(2000, 1000)]
    public void Add_ShouldRespectCapacity(int capacity, int expected)
    {
        var set = new SlidingSet<int>();

        Enumerable.Range(1, 1000)
            .ToList()
            .ForEach(x => set.Add(x, capacity));

        Assert.Equal(expected, set.Count);
    }

    [Fact]
    public void Add_ShouldRemoveOldestItem_IfExceedsCapacity()
    {
        var set = new SlidingSet<int>();

        Enumerable.Range(1, 20)
            .ToList()
            .ForEach(x => set.Add(x, 10));

        Assert.Contains(11, set);
        Assert.Contains(20, set);
        Assert.DoesNotContain(1, set);
        Assert.DoesNotContain(9, set);
    }
}
