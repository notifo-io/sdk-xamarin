using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;

namespace NotifoIO.SDK
{
	internal class NotifoMobilePush : INotifoMobilePush
	{
		private readonly HttpClient httpClient;
		private readonly AsyncRetryPolicy retryPolicy;

		private IPushEventsProvider pushEventsProvider;

		private string apiKey;
		private string baseUrl = "https://app.notifo.io";

		public NotifoMobilePush(HttpClient httpClient)
		{
			this.httpClient = httpClient;

			retryPolicy = Policy.Handle<WebException>()
			   .Or<HttpRequestException>()
			   .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300));
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

		private void PushEventsProvider_OnTokenRefresh(object sender, TokenRefreshEventArgs e)
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
				httpClient.DefaultRequestHeaders.Clear();
				httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

				retryPolicy.ExecuteAsync(async () => await httpClient.PostAsync(url, content));
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private JsonSerializerOptions JsonSerializerOptions() =>
			new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
	}
}
