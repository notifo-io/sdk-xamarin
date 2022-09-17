//// ==========================================================================
////  Notifo.io
//// ==========================================================================
////  Copyright (c) Sebastian Stehle
////  All rights reserved. Licensed under the MIT license.
//// ==========================================================================

//using System;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Moq;
//using Moq.AutoMock;
//using Moq.Contrib.HttpClient;
//using Notifo.SDK.NotifoMobilePush;
//using Notifo.SDK.PushEventProvider;
//using Notifo.SDK.Resources;
//using Notifo.SDK.Services;
//using Notifo.SDK.UnitTests.Mocks;
//using Xunit;

//namespace Notifo.SDK.UnitTests
//{
//    public class NotifoMobilePushTests
//    {
//        [Fact]
//        public void Notifo_ShouldSendTokenToBackend_IfTokenRefreshEventRaised()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetBaseUrl("https://test.com/")
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key");

//            eventsProvider.RaiseOnTokenRefreshEvent();

//            handler.VerifyRequest("https://test.com/api/me/mobilepush", Times.Once());

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.IsTokenRefreshed = true);
//        }

//        [Fact]
//        public void Notifo_ShouldNotSendTokenToBackend_IfTokenRefreshEventNotRaised()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key");

//            handler.VerifyAnyRequest(Times.Never());
//        }

//        [Fact]
//        public void Notifo_ShouldNotSendTokenToBackend_IfApiKeyIsNotPresent()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider);

//            eventsProvider.RaiseOnTokenRefreshEvent();

//            handler.VerifyAnyRequest(Times.Never());

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.IsTokenRefreshed = false);
//        }

//        [Fact]
//        public void Notifo_ShouldUseProperUrl_IfTokenRefreshEventRaised()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetBaseUrl("https://test.com/")
//                .SetApiKey("test api key");

//            eventsProvider.RaiseOnTokenRefreshEvent();

//            handler.VerifyRequest("https://test.com/api/me/mobilepush", Times.Once());
//        }

//        [Fact]
//        public void OnNotificationReceived_ShouldThrowException_IfEventsProviderNotSupplied()
//        {
//            var handler = new Mock<HttpMessageHandler>();
//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();

//            void EventHandler(object s, NotificationEventArgs e)
//            {
//            }

//            Action subscribe = () => notifoMobilePush.OnNotificationReceived += EventHandler;
//            subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventSubscribeException);

//            Action unsubscribe = () => notifoMobilePush.OnNotificationReceived -= EventHandler;
//            unsubscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventUnsubscribeException);
//        }

//        [Fact]
//        public void OnNotificationReceived_ShouldSubscribeAndUnsubscribe_IfEventsProviderSupplied()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush.SetPushEventsProvider(eventsProvider);

//            int invokeCount = 0;
//            void EventHandler(object s, NotificationEventArgs e) => invokeCount += 1;

//            notifoMobilePush.OnNotificationReceived += EventHandler;

//            eventsProvider.RaiseOnNotificationReceivedEvent();
//            invokeCount.Should().Be(1);

//            notifoMobilePush.OnNotificationReceived -= EventHandler;

//            eventsProvider.RaiseOnNotificationReceivedEvent();
//            invokeCount.Should().Be(1);
//        }

//        [Fact]
//        public void OnNotificationOpened_ShouldThrowException_IfEventsProviderNotSupplied()
//        {
//            var handler = new Mock<HttpMessageHandler>();
//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();

//            void EventHandler(object s, NotificationEventArgs e)
//            {
//            }

//            Action subscribe = () => notifoMobilePush.OnNotificationOpened += EventHandler;
//            subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationOpenedEventSubscribeException);

//            Action unsubscribe = () => notifoMobilePush.OnNotificationOpened -= EventHandler;
//            unsubscribe.Should().Throw<InvalidOperationException>(Strings.NotificationOpenedEventUnsubscribeException);
//        }

//        [Fact]
//        public void OnNotificationOpened_ShouldSubscribeAndUnsubscribe_IfEventsProviderSupplied()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush.SetPushEventsProvider(eventsProvider);

//            int invokeCount = 0;
//            void EventHandler(object s, NotificationEventArgs e) => invokeCount += 1;

//            notifoMobilePush.OnNotificationOpened += EventHandler;

//            eventsProvider.RaiseOnNotificationOpenedEvent();
//            invokeCount.Should().Be(1);

//            notifoMobilePush.OnNotificationOpened -= EventHandler;

//            eventsProvider.RaiseOnNotificationOpenedEvent();
//            invokeCount.Should().Be(1);
//        }

//        [Fact]
//        public void SetEventsProvider_ShouldNotSubscribeMutipleTimes_ForTheSameProvider()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetApiKey("test api key")
//                .SetPushEventsProvider(eventsProvider)
//                .SetPushEventsProvider(eventsProvider);

//            eventsProvider.RaiseOnTokenRefreshEvent();

//            handler.VerifyAnyRequest(Times.Once());
//        }

//        [Fact]
//        public void SetEventsProvider_ShouldUnsubscribeCurrentProvider_IfNewProviderSupplied()
//        {
//            var eventsProviderA = new EventsProviderMock();
//            var eventsProviderB = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetApiKey("test api key")
//                .SetPushEventsProvider(eventsProviderA);

//            int invokeOpenCount = 0;
//            void EventOpenHandler(object s, NotificationEventArgs e) => invokeOpenCount += 1;

//            int invokeReceivedCount = 0;
//            void EventReceivedHandler(object s, NotificationEventArgs e) => invokeReceivedCount += 1;

//            notifoMobilePush.OnNotificationOpened += EventOpenHandler;
//            notifoMobilePush.OnNotificationReceived += EventReceivedHandler;

//            notifoMobilePush.SetPushEventsProvider(eventsProviderB);
//            notifoMobilePush.OnNotificationOpened += EventOpenHandler;
//            notifoMobilePush.OnNotificationReceived += EventReceivedHandler;

//            eventsProviderA.RaiseOnTokenRefreshEvent();
//            eventsProviderB.RaiseOnTokenRefreshEvent();

//            handler.VerifyAnyRequest(Times.Once());

//            eventsProviderA.RaiseOnNotificationOpenedEvent();
//            eventsProviderB.RaiseOnNotificationOpenedEvent();

//            invokeOpenCount.Should().Be(1);

//            eventsProviderA.RaiseOnNotificationReceivedEvent();
//            eventsProviderB.RaiseOnNotificationReceivedEvent();

//            invokeReceivedCount.Should().Be(1);
//        }

//        [Fact]
//        public void Notifo_ShouldPersistNewToken_WhenTokenRefreshEventRaised()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider);

//            eventsProvider.RaiseOnTokenRefreshEvent();

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.Token = "test token");
//        }

//        [Fact]
//        public void Register_ShouldSetTokenRefreshedTrue_IfRefreshWasSuccessful()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.NoContent);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);
//            mocker.Setup<ISettings, string>(x => x.Token).Returns("test token");

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key")
//                .Register();

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.IsTokenRefreshed = true);
//        }

//        [Fact]
//        public void Register_ShouldSetTokenRefreshedFalse_IfBackendReturnedFailStatusCode()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.Unauthorized);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);
//            mocker.Setup<ISettings, string>(x => x.Token).Returns("test token");

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key")
//                .Register();

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.IsTokenRefreshed = false);
//        }

//        [Fact]
//        public void Register_ShouldSetTokenRefreshedFalse_IfHttpServiceThrewException()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .Throws(new Exception());

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);
//            mocker.Setup<ISettings, string>(x => x.Token).Returns("test token");

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key")
//                .Register();

//            var settingsMock = mocker.GetMock<ISettings>();
//            settingsMock.VerifySet(x => x.IsTokenRefreshed = false);
//        }

//        [Fact]
//        public void Register_ShouldNotRefreshToken_IfTokenIsEmpty()
//        {
//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .ReturnsResponse(HttpStatusCode.Unauthorized);

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);
//            mocker.Setup<IPushEventsProvider, string>(x => x.Token).Returns(string.Empty);
//            mocker.Setup<ISettings, string>(x => x.Token).Returns(string.Empty);

//            var eventsProvider = mocker.GetMock<IPushEventsProvider>().Object;

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key")
//                .Register();

//            handler.VerifyAnyRequest(Times.Never());
//        }

//        [Fact]
//        public void Register_ShouldNotRefreshToken_IfRefreshIsAlreadyRunning()
//        {
//            var eventsProvider = new EventsProviderMock();

//            var handler = new Mock<HttpMessageHandler>();
//            handler.SetupAnyRequest()
//                .Returns(async () =>
//                {
//                    await Task.Delay(100);
//                    return new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized };
//                });

//            var client = handler.CreateClient();

//            var mocker = new AutoMocker();
//            mocker.Use(client);
//            mocker.Setup<ISettings, string>(x => x.Token).Returns("test token");

//            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePushImplementation>();
//            notifoMobilePush
//                .SetPushEventsProvider(eventsProvider)
//                .SetApiKey("test api key");

//            eventsProvider.RaiseOnTokenRefreshEvent();
//            notifoMobilePush.Register();

//            handler.VerifyAnyRequest(Times.Once());
//        }
//    }
//}
