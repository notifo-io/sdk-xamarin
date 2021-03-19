// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
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

        private List<EventHandler<NotificationResponseEventArgs>> openedNotificationEvents;
        private List<EventHandler<NotificationDataEventArgs>> receivedNotificationEvents;

        public event EventHandler<NotificationDataEventArgs> OnNotificationReceived
        {
            add
            {
                if (pushEventsProvider == null)
                {
                    throw new InvalidOperationException(Strings.NotificationReceivedEventSubscribeException);
                }

                receivedNotificationEvents.Add(value);
                pushEventsProvider.OnNotificationReceived += value;
            }

            remove
            {
                if (pushEventsProvider == null)
                {
                    throw new InvalidOperationException(Strings.NotificationReceivedEventUnsubscribeException);
                }

                receivedNotificationEvents.Remove(value);
                pushEventsProvider.OnNotificationReceived -= value;
            }
        }

        public event EventHandler<NotificationResponseEventArgs> OnNotificationOpened
        {
            add
            {
                if (pushEventsProvider == null)
                {
                    throw new InvalidOperationException(Strings.NotificationOpenedEventSubscribeException);
                }

                openedNotificationEvents.Add(value);
                pushEventsProvider.OnNotificationOpened += value;
            }

            remove
            {
                if (pushEventsProvider == null)
                {
                    throw new InvalidOperationException(Strings.NotificationOpenedEventUnsubscribeException);
                }

                openedNotificationEvents.Remove(value);
                pushEventsProvider.OnNotificationOpened -= value;
            }
        }

        public NotifoMobilePush(IHttpService httpService)
        {
            this.httpService = httpService;

            openedNotificationEvents = new List<EventHandler<NotificationResponseEventArgs>>();
            receivedNotificationEvents = new List<EventHandler<NotificationDataEventArgs>>();
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
            if (this.pushEventsProvider == pushEventsProvider)
            {
                return this;
            }

            if (this.pushEventsProvider != null)
            {
                UnsubscribeEventsFromCurrentProvider();
            }

            this.pushEventsProvider = pushEventsProvider;
            this.pushEventsProvider.OnTokenRefresh += PushEventsProvider_OnTokenRefresh;

            return this;
        }

        private async void PushEventsProvider_OnTokenRefresh(object sender, TokenRefreshEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
            {
                return;
            }

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

        private void UnsubscribeEventsFromCurrentProvider()
        {
            if (pushEventsProvider == null)
            {
                return;
            }

            pushEventsProvider.OnTokenRefresh -= PushEventsProvider_OnTokenRefresh;

            foreach (var oe in openedNotificationEvents)
            {
                pushEventsProvider.OnNotificationOpened -= oe;
            }

            openedNotificationEvents.Clear();

            foreach (var re in receivedNotificationEvents)
            {
                pushEventsProvider.OnNotificationReceived -= re;
            }

            receivedNotificationEvents.Clear();
        }

        private JsonSerializerOptions JsonSerializerOptions() =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
    }
}
