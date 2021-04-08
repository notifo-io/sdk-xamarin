// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Moq.Contrib.HttpClient;
using Notifo.SDK.NotifoMobilePush;
using Xunit;

namespace Notifo.SDK.UnitTests
{
    public class NotifoClientProviderTests
    {
        [Fact]
        public void PostTokenAsync_ShouldThrow_IfApiKeyOrApiUrlNotSupplied()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.NoContent);

            var client = handler.CreateClient();

            var mocker = new AutoMocker();
            mocker.Use(client);

            var notifoMobilePush = mocker.CreateInstance<NotifoClientProvider>();

            Action action = () => notifoMobilePush.MobilePush.PostTokenAsync(new RegisterMobileTokenDto());

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void MobileClient_ShouldUseSameClient_IfApiKeyOrApiUrlNotChanged()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.NoContent);

            var client = handler.CreateClient();

            var mocker = new AutoMocker();
            mocker.Use(client);

            var notifoMobilePush = mocker.CreateInstance<NotifoClientProvider>();
            notifoMobilePush.ApiKey = "test api key";
            notifoMobilePush.ApiUrl = "https://test.com/";

            var mobilePush1 = notifoMobilePush.MobilePush;
            var mobilePush2 = notifoMobilePush.MobilePush;

            mobilePush1.Should().BeSameAs(mobilePush2);
        }

        [Fact]
        public void MobileClient_ShouldNotRebuildClient_IfApiKeyOrApiUrlNotChanged()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.NoContent);

            var client = handler.CreateClient();

            var mocker = new AutoMocker();
            mocker.Use(client);

            var notifoMobilePush = mocker.CreateInstance<NotifoClientProvider>();
            notifoMobilePush.ApiKey = "test api key";
            notifoMobilePush.ApiUrl = "https://test.com/";

            var mobilePush1 = notifoMobilePush.MobilePush;
            var mobilePush2 = notifoMobilePush.MobilePush;

            mobilePush1.Should().BeSameAs(mobilePush2);

            notifoMobilePush.ApiKey = "test api key";
            notifoMobilePush.ApiUrl = "https://test.com/";

            mobilePush2 = notifoMobilePush.MobilePush;

            mobilePush1.Should().BeSameAs(mobilePush2);
        }

        [Fact]
        public void MobileClient_ShouldRebuildClient_IfApiKeyOrApiUrlChanged()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.NoContent);

            var client = handler.CreateClient();

            var mocker = new AutoMocker();
            mocker.Use(client);

            var notifoMobilePush = mocker.CreateInstance<NotifoClientProvider>();
            notifoMobilePush.ApiKey = "test api key";
            notifoMobilePush.ApiUrl = "https://test.com/";

            var mobilePush1 = notifoMobilePush.MobilePush;

            notifoMobilePush.ApiKey = "test api key1";
            var mobilePush2 = notifoMobilePush.MobilePush;

            notifoMobilePush.ApiUrl = "https://test1.com/";
            var mobilePush3 = notifoMobilePush.MobilePush;

            mobilePush1.Should().NotBeSameAs(mobilePush2);
            mobilePush1.Should().NotBeSameAs(mobilePush3);
            mobilePush2.Should().NotBeSameAs(mobilePush3);
        }
    }
}
