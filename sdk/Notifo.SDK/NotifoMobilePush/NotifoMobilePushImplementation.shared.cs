// ==========================================================================
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
        private int refreshExecutingCount;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationOpened;

        /// <inheritdoc/>
        public IAppsClient Apps => clientProvider.Client.Apps;

        /// <inheritdoc/>
        public IConfigsClient Configs => clientProvider.Client.Configs;

        /// <inheritdoc/>
        public IEventsClient Events => clientProvider.Client.Events;

        /// <inheritdoc/>
        public ILogsClient Logs => clientProvider.Client.Logs;

        /// <inheritdoc/>
        public IMediaClient Media => clientProvider.Client.Media;

        /// <inheritdoc/>
        public IMobilePushClient MobilePush => clientProvider.Client.MobilePush;

        /// <inheritdoc/>
        public INotificationsClient Notifications => clientProvider.Client.Notifications;

        /// <inheritdoc/>
        public ITemplatesClient Templates => clientProvider.Client.Templates;

        /// <inheritdoc/>
        public ITopicsClient Topics => clientProvider.Client.Topics;

        /// <inheritdoc/>
        public IUsersClient Users => clientProvider.Client.Users;

        public NotifoMobilePushImplementation(Func<HttpClient> httpClientFactory, ISettings settings)
        {
            httpClient = httpClientFactory();
            this.settings = settings;

            clientProvider = new NotifoClientProvider(httpClientFactory);
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
                this.pushEventsProvider.OnTokenRefresh -= PushEventsProvider_OnTokenRefresh;
                this.pushEventsProvider.OnNotificationReceived -= PushEventsProvider_OnNotificationReceived;
                this.pushEventsProvider.OnNotificationOpened -= PushEventsProvider_OnNotificationOpened;
            }

            this.pushEventsProvider = pushEventsProvider;

            if (this.pushEventsProvider != null)
            {
                this.pushEventsProvider.OnTokenRefresh += PushEventsProvider_OnTokenRefresh;
                this.pushEventsProvider.OnNotificationReceived += PushEventsProvider_OnNotificationReceived;
                this.pushEventsProvider.OnNotificationOpened += PushEventsProvider_OnNotificationOpened;
            }

            return this;
        }

        public void Register()
        {
            bool notRefreshing = refreshExecutingCount == 0;
            if (notRefreshing)
            {
                string token =
                    string.IsNullOrWhiteSpace(pushEventsProvider?.Token)
                        ? settings.Token
                        : pushEventsProvider.Token;

                _ = EnsureTokenRefreshedAsync(token);
                return;
            }
        }

        private async Task RegisterCoreAsync()
        {
            try
            {
                var token = await settings.GetTokenAsync();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    await MobilePush.DeleteTokenAsync(token);
                }

                settings.Clear();
            }
            catch (Exception ex)
            {
                Log.Error(ex, Strings.TokenRemoveFailException);
            }
        }

        public void Unregister()
        {
            _ = UnregisterCoreAsync();
        }

        private async Task UnregisterCoreAsync()
        {
            try
            {
                var token = await settings.GetTokenAsync();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    await MobilePush.DeleteTokenAsync(token);
                }

                settings.Clear();
            }
            catch (Exception ex)
            {
                Log.Error(ex, Strings.TokenRemoveFailException);
            }
        }

        private void PushEventsProvider_OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            // Forward the event to the application.
            OnNotificationReceived?.Invoke(sender, e);

            // we are tracking notifications only for Android here because it is the entry point for all notifications that the Android device receives
            // this is not the case for iOS where the entry point is in Notification Service Extension
            if (DevicePlatform.Android == DeviceInfo.Platform && !string.IsNullOrWhiteSpace(e.Notification.TrackingUrl))
            {
                _ = TrackNotificationsAsync(e.Notification);
            }
        }

        private void PushEventsProvider_OnNotificationOpened(object sender, NotificationEventArgs e)
        {
            // Forward the event to the application.
            OnNotificationOpened?.Invoke(sender, e);
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

                await MobilePush.PostTokenAsync(registerMobileTokenDto);

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

        private async Task<ICollection<NotificationDto>> GetPendingNotificationsAsync(int take, TimeSpan period)
        {
            try
            {
                var allNotifications = await Notifications.GetNotificationsAsync(take: take);
                var seenNotifications = await settings.GetSeenNotificationIdsAsync();

                var utcNow = DateTimeOffset.UtcNow;

                var pendingNotifications = allNotifications
                    .Items
                    .Where(x => !seenNotifications.Contains(x.Id))
                    .Where(x => (utcNow - x.Created.UtcDateTime) <= period)
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

        private async Task TrackNotificationsAsync(params NotificationDto[] notifications)
        {
            try
            {
                var seenIds = notifications.Select(x => x.Id).ToArray();

                await settings.SetSeenNotificationIdsAsync(seenIds);

                var trackNotificationDto = new TrackNotificationDto
                {
                    Seen = seenIds,
                    DeviceIdentifier = await settings.GetTokenAsync()
                };

                await Notifications.ConfirmAsync(trackNotificationDto);
            }
            catch (Exception ex)
            {
                Log.Error(Strings.TrackingException, ex);
            }
        }
    }
}
