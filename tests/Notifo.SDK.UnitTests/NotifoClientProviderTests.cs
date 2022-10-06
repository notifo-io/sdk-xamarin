// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using Notifo.SDK.NotifoMobilePush;
using Xunit;

namespace Notifo.SDK.UnitTests
{
    public class NotifoClientProviderTests
    {
        [Fact]
        public void Client_ShouldThrow_IfApiKeyOrApiUrlNotSupplied()
        {
            var provider = new NotifoClientProvider(() => new HttpClient());

            Assert.Throws<InvalidOperationException>(() => provider.Client);
        }

        [Fact]
        public void Client_ShouldUseSameClient_IfApiKeyOrApiUrlNotChanged()
        {
            var provider = new NotifoClientProvider(() => new HttpClient())
            {
                ApiKey = "test api key",
                ApiUrl = "https://url1.com/"
            };

            var client1 = provider.Client;
            var client2 = provider.Client;

            Assert.Same(client1, client2);
        }

        [Fact]
        public void Client_ShouldNotRebuildClient_IfApiKeyOrApiUrlNotChanged()
        {
            var provider = new NotifoClientProvider(() => new HttpClient())
            {
                ApiKey = "key1",
                ApiUrl = "https://url1.com/"
            };

            var client1 = provider.Client;

            provider.ApiKey = "key1";
            provider.ApiUrl = "https://url1.com";

            var client2 = provider.Client;

            Assert.Same(client1, client2);
        }

        [Fact]
        public void Client_ShouldRebuildClient_IfApiKeyChanged()
        {
            var provider = new NotifoClientProvider(() => new HttpClient())
            {
                ApiKey = "key1",
                ApiUrl = "https://url1.com/"
            };

            var client1 = provider.Client;

            provider.ApiKey = "key2";

            var client2 = provider.Client;

            Assert.NotSame(client1, client2);
        }

        [Fact]
        public void Client_ShouldRebuildClient_IfApiUrlChanged()
        {
            var provider = new NotifoClientProvider(() => new HttpClient())
            {
                ApiKey = "key1",
                ApiUrl = "https://url1.com/"
            };

            var client1 = provider.Client;

            provider.ApiUrl = "https://url2.com/";

            var client2 = provider.Client;

            Assert.NotSame(client1, client2);
        }
    }
}
