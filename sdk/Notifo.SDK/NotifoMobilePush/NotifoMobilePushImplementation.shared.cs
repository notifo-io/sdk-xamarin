﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.Extensions;
using Notifo.SDK.PushEventProvider;
using Notifo.SDK.Resources;
using Notifo.SDK.Services;
using Serilog;
using Xamarin.Essentials;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : INotifoMobilePush
    {
        private readonly HttpClient httpClient;
        private readonly ISettings settings;
        private readonly NotifoClientProvider clientProvider;

        private IPushEventsProvider? pushEventsProvider;

        private List<EventHandler<NotificationResponseEventArgs>> openedNotificationEvents;
        private List<EventHandler<NotificationDataEventArgs>> receivedNotificationEvents;

        private int refreshExecutingCount;

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

        public NotifoMobilePushImplementation(HttpClient httpClient, ISettings settings)
        {
            this.httpClient = httpClient;
            this.settings = settings;

            clientProvider = new NotifoClientProvider(httpClient);

            openedNotificationEvents = new List<EventHandler<NotificationResponseEventArgs>>();
            receivedNotificationEvents = new List<EventHandler<NotificationDataEventArgs>>();

            refreshExecutingCount = 0;
        }

        public INotifoMobilePush SetApiKey(string apiKey)
        {
            clientProvider.ApiKey = apiKey;

            return this;
        }

        public INotifoMobilePush SetBaseUrl(string baseUrl)
        {
            clientProvider.ApiUrl = baseUrl;

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
            this.pushEventsProvider.OnNotificationReceived += PushEventsProvider_OnNotificationReceived;

            return this;
        }

        public void Register()
        {
            bool notRefreshing = refreshExecutingCount == 0;
            if (notRefreshing)
            {
                _ = EnsureTokenRefreshedAsync(settings.Token);
            }
        }

        private void PushEventsProvider_OnNotificationReceived(object sender, NotificationDataEventArgs e)
        {
            // we are tracking notifications only for Android here because it is the entry point for all notifications that the Android device receives
            // this is not the case for iOS where the entry point is in Notification Service Extension
            if (DevicePlatform.Android == DeviceInfo.Platform)
            {
                if (e.Data.ContainsKey(Constants.IdKey) && e.Data.ContainsKey(Constants.TrackingUrlKey))
                {
                    var notificationId = Guid.Parse(e.Data[Constants.IdKey].ToString());
                    var trackingUrl = e.Data[Constants.TrackingUrlKey].ToString();

                    _ = TrackNotificationAsync(notificationId, trackingUrl);
                }
            }
        }

        private void PushEventsProvider_OnTokenRefresh(object sender, TokenRefreshEventArgs e)
        {
            _ = EnsureTokenRefreshedAsync(e.Token);
        }

        private async Task EnsureTokenRefreshedAsync(string token)
        {
            try
            {
                Log.Debug(Strings.TokenRefreshStartExecutingCount, refreshExecutingCount);

                Interlocked.Increment(ref refreshExecutingCount);

                bool alreadyRefreshed = settings.Token == token && settings.IsTokenRefreshed;
                if (alreadyRefreshed || string.IsNullOrWhiteSpace(token))
                {
                    return;
                }

                settings.Token = token;
                settings.IsTokenRefreshed = false;

                var registerMobileTokenDto = new RegisterMobileTokenDto
                {
                    Token = token,
                    DeviceType = DeviceInfo.Platform.ToMobileDeviceType()
                };

                await clientProvider.MobilePush.PostTokenAsync(registerMobileTokenDto);

                settings.IsTokenRefreshed = true;
                Log.Debug(Strings.TokenRefreshSuccess, token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Strings.TokenRefreshFailException);
            }
            finally
            {
                Interlocked.Decrement(ref refreshExecutingCount);

                Log.Debug(Strings.TokenRefreshEndExecutingCount, refreshExecutingCount);
            }
        }

        private void UnsubscribeEventsFromCurrentProvider()
        {
            if (pushEventsProvider == null)
            {
                return;
            }

            pushEventsProvider.OnTokenRefresh -= PushEventsProvider_OnTokenRefresh;
            pushEventsProvider.OnNotificationReceived -= PushEventsProvider_OnNotificationReceived;

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

        private async Task<ICollection<NotificationDto>> GetPendingNotificationsAsync()
        {
            try
            {
                var allNotifications = await clientProvider.Notifications.GetNotificationsAsync();
                var pendingNotifications = allNotifications
                    .Items
                    .Where(x => !settings.IsNotificationSeen(x.Id) && !x.IsSeen)
                    .OrderBy(x => x.Created)
                    .ToArray();

                Log.Debug(Strings.PendingNotificationsCount, pendingNotifications.Length);

                return pendingNotifications;
            }
            catch (Exception ex)
            {
                Log.Error(Strings.NotificationsRetrieveException, ex);
            }

            return new NotificationDto[] { };
        }

        private async Task TrackNotificationAsync(Guid notificationId, string trackingUrl)
        {
            Log.Debug(Strings.TrackingUrl, trackingUrl);

            try
            {
                settings.TrackNotification(notificationId);

                var response = await httpClient.GetAsync(trackingUrl);
                Log.Debug(Strings.TrackingResponseCode, response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.Error(Strings.TrackingException, ex);
            }
        }

        private async Task TrackNotificationsAsync(IEnumerable<NotificationDto> notifications)
        {
            try
            {
                var seenIds = notifications.Select(x => x.Id).ToArray();

                settings.TrackNotifications(seenIds);

                var trackNotificationDto = new TrackNotificationDto
                {
                    Seen = seenIds,
                    DeviceIdentifier = settings.Token
                };

                await clientProvider.Notifications.ConfirmAsync(trackNotificationDto);
            }
            catch (Exception ex)
            {
                Log.Error(Strings.TrackingException, ex);
            }
        }
    }
}