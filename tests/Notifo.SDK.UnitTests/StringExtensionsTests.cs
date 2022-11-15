// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Linq;
using Notifo.SDK.Extensions;
using Xunit;

namespace Notifo.SDK.UnitTests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Should_calculate_base64()
        {
            var input = "https://notifo.io";

            var result = input.ToBase64();

            Assert.NotNull(result);
        }

        [Fact]
        public void Should_calculate_sha256()
        {
            var input = "https://notifo.io";

            var result = input.Sha256();

            Assert.True(result.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')));
        }

        [Fact]
        public void Should_append_query_to_empty_query()
        {
            var input = "https://notifo.io";

            var result = input.AppendQueries("q1", "1");

            Assert.Equal("https://notifo.io?q1=1", result);
        }

        [Fact]
        public void Should_append_query_to_existing_query()
        {
            var input = "https://notifo.io?key=value";

            var result = input.AppendQueries("q1", "1");

            Assert.Equal("https://notifo.io?key=value&q1=1", result);
        }

        [Fact]
        public void Should_append_queries_to_empty_query()
        {
            var input = "https://notifo.io";

            var result = input.AppendQueries("q1", "1", "q2", "2");

            Assert.Equal("https://notifo.io?q1=1&q2=2", result);
        }

        [Fact]
        public void Should_append_queries_to_existing_query()
        {
            var input = "https://notifo.io?key=value";

            var result = input.AppendQueries("q1", "1", "q2", "2");

            Assert.Equal("https://notifo.io?key=value&q1=1&q2=2", result);
        }

        [Fact]
        public void Should_not_append_null_values()
        {
            var input = "https://notifo.io?key=value";

            var result = input.AppendQueries("q1", null, "q2", null);

            Assert.Equal(input, result);
        }

        [Fact]
        public void Should_not_append_null_keys()
        {
            var input = "https://notifo.io?key=value";

            var result = input.AppendQueries(null, "1", null, "2");

            Assert.Equal(input, result);
        }
    }
}
