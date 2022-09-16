// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : INotifoMobilePush
    {
        private readonly ISeenNotificationsStore seenNotificationsStore;
        private readonly ICommandQueue commandQueue;
        private readonly NotifoClientProvider clientProvider;
        private IPushEventsProvider? pushEventsProvider;
        private string token;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationOpened;

        /// <inheritdoc/>
        public event EventHandler<NotificationErrorEventArgs> OnError;

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

        public NotifoMobilePushImplementation(Func<HttpClient> httpClientFactory, ISeenNotificationsStore seenNotificationsStore, ICommandQueue commandQueue)
        {
            this.seenNotificationsStore = seenNotificationsStore;
            this.commandQueue = commandQueue;
            this.commandQueue.OnError += CommandQueue_OnError;
            this.clientProvider = new NotifoClientProvider(httpClientFactory);

            SetupPlatform();
        }

        partial void SetupPlatform();

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
                this.pushEventsProvider.OnError -= PushEventsProvider_OnError;
            }

            this.pushEventsProvider = pushEventsProvider;

            if (this.pushEventsProvider != null)
            {
                this.pushEventsProvider.OnTokenRefresh += PushEventsProvider_OnTokenRefresh;
                this.pushEventsProvider.OnNotificationReceived += PushEventsProvider_OnNotificationReceived;
                this.pushEventsProvider.OnNotificationOpened += PushEventsProvider_OnNotificationOpened;
                this.pushEventsProvider.OnError += PushEventsProvider_OnError;
            }

            UpdateToken(pushEventsProvider.Token);

            return this;
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            commandQueue.Run(new TokenRegisterCommand { Token = token });
        }

        public void Unregister()
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            commandQueue.Run(new TokenUnregisterCommand { Token = token });
        }

        private void PushEventsProvider_OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            // Forward the event to the application.
            OnNotificationReceived?.Invoke(sender, e);
        }

        private void PushEventsProvider_OnNotificationOpened(object sender, NotificationEventArgs e)
        {
            // Forward the event to the application.
            OnNotificationOpened?.Invoke(sender, e);
        }

        private void PushEventsProvider_OnError(object sender, NotificationErrorEventArgs e)
        {
            // Forward the event to the application.
            OnError?.Invoke(sender, e);
        }

        private void CommandQueue_OnError(object sender, NotificationErrorEventArgs e)
        {
            // Forward the event to the application.
            OnError?.Invoke(sender, e);
        }

        private void PushEventsProvider_OnTokenRefresh(object sender, TokenRefreshEventArgs e)
        {
            UpdateToken(e.Token);
        }

        private void UpdateToken(string newToken)
        {
            if (!string.Equals(newToken, token))
            {
                token = newToken;

                Register();
            }
        }

        public void RaiseError(string error, Exception? exception, object? source)
        {
            OnError?.Invoke(this, new NotificationErrorEventArgs(error, exception, source));
        }
    }
}
