using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
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



		private class EventsProviderMock : IPushEventsProvider
		{
			public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

			protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args)
			{
				var handler = OnTokenRefresh;
				handler?.Invoke(this, args);
			}

			public void RaiseOnTokenRefreshEvent()
			{
				var args = new TokenRefreshEventArgs
				{
					Token = "test token"
				};
				OnRefreshTokenEvent(args);
			}
		}
	}
}
