using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NotifoIO.SDK.Resources;
using Xunit;

namespace NotifoIO.SDK.UnitTests
{
	public class NotifoMobilePushTests
	{
		[Fact]
		public void Notifo_ShouldSendTokenToBackend_IfTokenRefreshEventRaised()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handlerMock
			   .Protected()
			   .Setup<Task<HttpResponseMessage>>
			   (
				  "SendAsync",
				  ItExpr.IsAny<HttpRequestMessage>(),
				  ItExpr.IsAny<CancellationToken>()
			   )
			   .ReturnsAsync(new HttpResponseMessage()
			   {
				   StatusCode = HttpStatusCode.OK,
				   Content = new StringContent("OK"),
			   })
			   .Verifiable();

			var httpClient = new HttpClient(handlerMock.Object);
			var eventsProvider = new EventsProviderMock();

			var notifoMobilePush = new NotifoMobilePush(httpClient);
			notifoMobilePush
				.SetPushEventsProvider(eventsProvider)
				.SetApiKey("test api key");

			eventsProvider.RaiseOnTokenRefreshEvent();

			handlerMock
				.Protected()
				.Verify
				(
				   "SendAsync",
				   Times.Exactly(1),
				   ItExpr.Is<HttpRequestMessage>(req =>
					  req.Method == HttpMethod.Post && req.RequestUri.ToString().EndsWith("/api/mobilepush")
				   ),
				   ItExpr.IsAny<CancellationToken>()
				);
		}


		[Fact]
		public void Notifo_ShouldNotSendTokenToBackend_IfTokenRefreshEventNotRaised()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handlerMock
			   .Protected()
			   .Setup<Task<HttpResponseMessage>>
			   (
				  "SendAsync",
				  ItExpr.IsAny<HttpRequestMessage>(),
				  ItExpr.IsAny<CancellationToken>()
			   )
			   .Verifiable();

			var httpClient = new HttpClient(handlerMock.Object);
			var eventsProvider = new EventsProviderMock();

			var notifoMobilePush = new NotifoMobilePush(httpClient);
			notifoMobilePush
				.SetPushEventsProvider(eventsProvider)
				.SetApiKey("test api key");

			handlerMock
				.Protected()
				.Verify
				(
				   "SendAsync",
				   Times.Never(),
				   ItExpr.IsAny<HttpRequestMessage>(),
				   ItExpr.IsAny<CancellationToken>()
				);
		}

		[Fact]
		public void Notifo_ShouldNotSendTokenToBackend_IfApiKeyIsNotPresent()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handlerMock
			   .Protected()
			   .Setup<Task<HttpResponseMessage>>
			   (
				  "SendAsync",
				  ItExpr.IsAny<HttpRequestMessage>(),
				  ItExpr.IsAny<CancellationToken>()
			   )
			   .Verifiable();

			var httpClient = new HttpClient(handlerMock.Object);
			var eventsProvider = new EventsProviderMock();

			var notifoMobilePush = new NotifoMobilePush(httpClient);
			notifoMobilePush
				.SetPushEventsProvider(eventsProvider);

			handlerMock
				.Protected()
				.Verify
				(
				   "SendAsync",
				   Times.Never(),
				   ItExpr.IsAny<HttpRequestMessage>(),
				   ItExpr.IsAny<CancellationToken>()
				);
		}

		[Fact]
		public void Notifo_ShouldUseProperUrl_IfTokenRefreshEventRaised()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handlerMock
			   .Protected()
			   .Setup<Task<HttpResponseMessage>>
			   (
				  "SendAsync",
				  ItExpr.IsAny<HttpRequestMessage>(),
				  ItExpr.IsAny<CancellationToken>()
			   )
			   .ReturnsAsync(new HttpResponseMessage()
			   {
				   StatusCode = HttpStatusCode.OK,
				   Content = new StringContent("OK"),
			   })
			   .Verifiable();

			var httpClient = new HttpClient(handlerMock.Object);
			var eventsProvider = new EventsProviderMock();

			var notifoMobilePush = new NotifoMobilePush(httpClient);
			notifoMobilePush
				.SetPushEventsProvider(eventsProvider)
				.SetBaseUrl("https://test.com/")
				.SetApiKey("test api key");


			eventsProvider.RaiseOnTokenRefreshEvent();

			handlerMock
				.Protected()
				.Verify
				(
				   "SendAsync",
				   Times.Exactly(1),
				   ItExpr.Is<HttpRequestMessage>(req =>
					  req.Method == HttpMethod.Post && req.RequestUri.ToString() == "https://test.com/api/mobilepush"
				   ),
				   ItExpr.IsAny<CancellationToken>()
				);
		}

		[Fact]
		public void OnNotificationReceived_ShouldThrowException_IfEventsProviderNotSupplied()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(handlerMock.Object);

			var notifoMobilePush = new NotifoMobilePush(httpClient);

			Action subscribe = () => notifoMobilePush.OnNotificationReceived += (s, e) => { };
			subscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventSubscribeException);

			Action unsubscribe = () => notifoMobilePush.OnNotificationReceived -= (s, e) => { };
			unsubscribe.Should().Throw<InvalidOperationException>(Strings.NotificationReceivedEventUnsubscribeException);
		}

		[Fact]
		public void OnNotificationReceived_ShouldSubscribeAndUnsubscribe_IfEventsProviderSupplied()
		{
			var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			var httpClient = new HttpClient(handlerMock.Object);
			var eventsProvider = new EventsProviderMock();

			var notifoMobilePush = new NotifoMobilePush(httpClient);
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



		private class EventsProviderMock : IPushEventsProvider
		{
			public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
			public event EventHandler<NotificationDataEventArgs> OnNotificationReceived;

			protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args)
			{
				var handler = OnTokenRefresh;
				handler?.Invoke(this, args);
			}

			public void RaiseOnTokenRefreshEvent()
			{
				var args = new TokenRefreshEventArgs("test token");
				OnRefreshTokenEvent(args);
			}

			protected virtual void OnNotificationReceivedEvent(NotificationDataEventArgs args)
			{
				var handler = OnNotificationReceived;
				handler?.Invoke(this, args);
			}

			public void RaiseOnNotificationReceivedEvent()
			{
				var args = new NotificationDataEventArgs(new Notification());
				OnNotificationReceivedEvent(args);
			}
		}
	}
}
