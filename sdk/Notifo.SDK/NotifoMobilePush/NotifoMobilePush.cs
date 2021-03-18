using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using NotifoIO.SDK.Resources;
using Serilog;

namespace NotifoIO.SDK
{
	internal class NotifoMobilePush : INotifoMobilePush
	{
		private readonly IHttpService httpService;

		private string? apiKey;
		private string baseUrl = "https://app.notifo.io";
		private IPushEventsProvider? pushEventsProvider;

		public event EventHandler<NotificationDataEventArgs> OnNotificationReceived
		{
			add
			{
				if (pushEventsProvider == null)
					throw new InvalidOperationException(Strings.NotificationReceivedEventSubscribeException);

				pushEventsProvider.OnNotificationReceived += value;
			}

			remove
			{
				if (pushEventsProvider == null)
					throw new InvalidOperationException(Strings.NotificationReceivedEventUnsubscribeException);

				pushEventsProvider.OnNotificationReceived -= value;
			}
		}

		public event EventHandler<NotificationResponseEventArgs> OnNotificationOpened
		{
			add
			{
				if (pushEventsProvider == null)
					throw new InvalidOperationException(Strings.NotificationOpenedEventSubscribeException);

				pushEventsProvider.OnNotificationOpened += value;
			}

			remove
			{
				if (pushEventsProvider == null)
					throw new InvalidOperationException(Strings.NotificationOpenedEventUnsubscribeException);

				pushEventsProvider.OnNotificationOpened -= value;
			}
		}

		public NotifoMobilePush(IHttpService httpService)
		{
			this.httpService = httpService;
		}

		public INotifoMobilePush SetApiKey(string apiKey)
		{
			this.apiKey = apiKey;
			return this;
		}

		public INotifoMobilePush SetBaseUrl(string baseUrl)
		{
			this.baseUrl = baseUrl.TrimEnd('/');
			return this;
		}

		public INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider)
		{
			this.pushEventsProvider = pushEventsProvider;
			this.pushEventsProvider.OnTokenRefresh += PushEventsProvider_OnTokenRefresh;

			return this;
		}

		private async void PushEventsProvider_OnTokenRefresh(object sender, TokenRefreshEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
				return;

			string url = $"{baseUrl}/api/mobilepush";

			var payload = new
			{
				Token = e.Token,
			};

			var content = new StringContent(JsonSerializer.Serialize(payload, JsonSerializerOptions()), Encoding.UTF8, "application/json");
			try
			{
				var response = await httpService.PostAsync(url, content, apiKey!);
				if (response.IsSuccessStatusCode)
				{
					Log.Debug(Strings.TokenRefreshSuccess, e.Token);
				}
				else
				{
					Log.Error(Strings.TokenRefreshFailStatusCode, response.StatusCode);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, Strings.TokenRefreshFailException);
			}
		}

		private JsonSerializerOptions JsonSerializerOptions() =>
			new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
	}
}
