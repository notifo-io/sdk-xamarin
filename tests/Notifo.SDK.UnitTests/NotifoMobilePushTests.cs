// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NotifoIO.SDK.Resources;
using Xunit;

namespace NotifoIO.SDK.UnitTests
{
    public class NotifoMobilePushTests
    {
        [Fact]
        public void Notifo_ShouldSendTokenToBackend_IfTokenRefreshEventRaised()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetPushEventsProvider(eventsProvider)
                .SetApiKey("test api key");

            eventsProvider.RaiseOnTokenRefreshEvent();

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.Is<string>(url => url.EndsWith("/api/mobilepush", StringComparison.InvariantCultureIgnoreCase)),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Once()
                );
        }

        [Fact]
        public void Notifo_ShouldNotSendTokenToBackend_IfTokenRefreshEventNotRaised()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetPushEventsProvider(eventsProvider)
                .SetApiKey("test api key");

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.IsAny<string>(),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Never()
                );
        }

        [Fact]
        public void Notifo_ShouldNotSendTokenToBackend_IfApiKeyIsNotPresent()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetPushEventsProvider(eventsProvider);

            eventsProvider.RaiseOnTokenRefreshEvent();

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.IsAny<string>(),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Never()
                );
        }

        [Fact]
        public void Notifo_ShouldUseProperUrl_IfTokenRefreshEventRaised()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetPushEventsProvider(eventsProvider)
                .SetBaseUrl("https://test.com/")
                .SetApiKey("test api key");

            eventsProvider.RaiseOnTokenRefreshEvent();

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.Is<string>(url => url == "https://test.com/api/mobilepush"),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Once()
                );
        }

        [Fact]
        public void OnNotificationReceived_ShouldThrowException_IfEventsProviderNotSupplied()
        {
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();

            void EventHandler(object s, NotificationDataEventArgs e)
            {
            }

            Action subscribe = () => notifoMobilePush.OnNotificationReceived += EventHandler;
            subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventSubscribeException);

            Action unsubscribe = () => notifoMobilePush.OnNotificationReceived -= EventHandler;
            unsubscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventUnsubscribeException);
        }

        [Fact]
        public void OnNotificationReceived_ShouldSubscribeAndUnsubscribe_IfEventsProviderSupplied()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush.SetPushEventsProvider(eventsProvider);

            int invokeCount = 0;
            void EventHandler(object s, NotificationDataEventArgs e) => invokeCount += 1;

            notifoMobilePush.OnNotificationReceived += EventHandler;

            eventsProvider.RaiseOnNotificationReceivedEvent();
            invokeCount.Should().Be(1);

            notifoMobilePush.OnNotificationReceived -= EventHandler;

            eventsProvider.RaiseOnNotificationReceivedEvent();
            invokeCount.Should().Be(1);
        }

        [Fact]
        public void OnNotificationOpened_ShouldThrowException_IfEventsProviderNotSupplied()
        {
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();

            void EventHandler(object s, NotificationResponseEventArgs e)
            {
            }

            Action subscribe = () => notifoMobilePush.OnNotificationOpened += EventHandler;
            subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationOpenedEventSubscribeException);

            Action unsubscribe = () => notifoMobilePush.OnNotificationOpened -= EventHandler;
            unsubscribe.Should().Throw<InvalidOperationException>(Strings.NotificationOpenedEventUnsubscribeException);
        }

        [Fact]
        public void OnNotificationOpened_ShouldSubscribeAndUnsubscribe_IfEventsProviderSupplied()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush.SetPushEventsProvider(eventsProvider);

            int invokeCount = 0;
            void EventHandler(object s, NotificationResponseEventArgs e) => invokeCount += 1;

            notifoMobilePush.OnNotificationOpened += EventHandler;

            eventsProvider.RaiseOnNotificationOpenedEvent();
            invokeCount.Should().Be(1);

            notifoMobilePush.OnNotificationOpened -= EventHandler;

            eventsProvider.RaiseOnNotificationOpenedEvent();
            invokeCount.Should().Be(1);
        }

        [Fact]
        public void SetEventsProvider_ShouldNotSubscribeMutipleTimes_ForTheSameProvider()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetApiKey("test api key")
                .SetPushEventsProvider(eventsProvider)
                .SetPushEventsProvider(eventsProvider);

            eventsProvider.RaiseOnTokenRefreshEvent();

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.IsAny<string>(),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Once()
                );
        }

        [Fact]
        public void SetEventsProvider_ShouldUnsubscribeCurrentProvider_IfNewProviderSupplied()
        {
            var eventsProviderA = new EventsProviderMock();
            var eventsProviderB = new EventsProviderMock();

            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetApiKey("test api key")
                .SetPushEventsProvider(eventsProviderA);

            int invokeOpenCount = 0;
            void EventOpenHandler(object s, NotificationResponseEventArgs e) => invokeOpenCount += 1;

            int invokeReceivedCount = 0;
            void EventReceivedHandler(object s, NotificationDataEventArgs e) => invokeReceivedCount += 1;

            notifoMobilePush.OnNotificationOpened += EventOpenHandler;
            notifoMobilePush.OnNotificationReceived += EventReceivedHandler;

            notifoMobilePush.SetPushEventsProvider(eventsProviderB);
            notifoMobilePush.OnNotificationOpened += EventOpenHandler;
            notifoMobilePush.OnNotificationReceived += EventReceivedHandler;

            eventsProviderA.RaiseOnTokenRefreshEvent();
            eventsProviderB.RaiseOnTokenRefreshEvent();

            var httpServiceMock = mocker.GetMock<IHttpService>();
            httpServiceMock
                .Verify(
                    x => x.PostAsync(
                            It.IsAny<string>(),
                            It.IsAny<HttpContent>(),
                            It.IsAny<string>()
                        ),
                    Times.Once()
                );

            eventsProviderA.RaiseOnNotificationOpenedEvent();
            eventsProviderB.RaiseOnNotificationOpenedEvent();

            invokeOpenCount.Should().Be(1);

            eventsProviderA.RaiseOnNotificationReceivedEvent();
            eventsProviderB.RaiseOnNotificationReceivedEvent();

            invokeReceivedCount.Should().Be(1);
        }

        [Fact]
        public void Notifo_ShouldPersistNewToken_WhenTokenRefreshEventRaised()
        {
            var eventsProvider = new EventsProviderMock();
            var mocker = new AutoMocker();
            var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();
            notifoMobilePush
                .SetPushEventsProvider(eventsProvider);

            eventsProvider.RaiseOnTokenRefreshEvent();

            var settingsMock = mocker.GetMock<ISettings>();
            settingsMock.VerifySet(x => x.Token = "test token");
        }
    }
}
