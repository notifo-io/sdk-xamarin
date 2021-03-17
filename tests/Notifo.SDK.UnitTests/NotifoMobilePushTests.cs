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
				.Verify
				(
					x => x.PostAsync
						(
							It.Is<string>(url => url.EndsWith("/api/mobilepush")),
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
				.Verify
				(
					x => x.PostAsync
						(
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
				.Verify
				(
					x => x.PostAsync
						(
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
				.Verify
				(
					x => x.PostAsync
						(
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

			Action subscribe = () => notifoMobilePush.OnNotificationReceived += (s, e) => { };
			subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventSubscribeException);

			Action unsubscribe = () => notifoMobilePush.OnNotificationReceived -= (s, e) => { };
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
			void eventHandler(object s, NotificationDataEventArgs e) => invokeCount += 1;

			notifoMobilePush.OnNotificationReceived += eventHandler;

			eventsProvider.RaiseOnNotificationReceivedEvent();
			invokeCount.Should().Be(1);

			notifoMobilePush.OnNotificationReceived -= eventHandler;

			eventsProvider.RaiseOnNotificationReceivedEvent();
			invokeCount.Should().Be(1);
		}

		[Fact]
		public void OnNotificationOpened_ShouldThrowException_IfEventsProviderNotSupplied()
		{
			var mocker = new AutoMocker();
			var notifoMobilePush = mocker.CreateInstance<NotifoMobilePush>();

			Action subscribe = () => notifoMobilePush.OnNotificationOpened += (s, e) => { };
			subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationOpenedEventSubscribeException);

			Action unsubscribe = () => notifoMobilePush.OnNotificationOpened -= (s, e) => { };
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
			void eventHandler(object s, NotificationResponseEventArgs e) => invokeCount += 1;

			notifoMobilePush.OnNotificationOpened += eventHandler;

			eventsProvider.RaiseOnNotificationOpenedEvent();
			invokeCount.Should().Be(1);

			notifoMobilePush.OnNotificationOpened -= eventHandler;

			eventsProvider.RaiseOnNotificationOpenedEvent();
			invokeCount.Should().Be(1);
		}		
	}
}
