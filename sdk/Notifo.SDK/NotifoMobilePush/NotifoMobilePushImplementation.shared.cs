// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.PushEventProvider;
using Serilog;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation : INotifoMobilePush
    {
        private readonly ISeenNotificationsStore seenNotificationsStore;
        private readonly ICommandQueue commandQueue;
        private readonly ICommandStore commandStore;
        private readonly ICredentialsStore credentialsStore;
        private readonly NotifoClientProvider clientProvider;
        private IPushEventsProvider? pushEventsProvider;
        private string? token;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationOpened;

        /// <inheritdoc/>
        public event EventHandler<NotificationErrorEventArgs> OnError;

        /// <inheritdoc/>
        public ApiVersion ApiVersion { get; private set; }

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

        /// <inheritdoc/>
        public IEmailTemplatesClient EmailTemplates => clientProvider.Client.EmailTemplates;

        /// <inheritdoc/>
        public IMessagingTemplatesClient MessagingTemplates => clientProvider.Client.MessagingTemplates;

        /// <inheritdoc/>
        public IPingClient Ping => clientProvider.Client.Ping;

        /// <inheritdoc/>
        public ISmsTemplatesClient SmsTemplates => clientProvider.Client.SmsTemplates;

        /// <inheritdoc/>
        public ISystemUsersClient SystemUsers => clientProvider.Client.SystemUsers;

        /// <inheritdoc/>
        public IUserClient User => clientProvider.Client.User;

        /// <inheritdoc/>
        public bool IsConfigured => clientProvider.IsConfigured;

        public NotifoMobilePushImplementation(
            Func<HttpClient> httpClientFactory,
            ISeenNotificationsStore seenNotificationsStore,
            ICommandQueue commandQueue,
            ICommandStore commandStore,
            ICredentialsStore credentialsStore)
        {
            this.seenNotificationsStore = seenNotificationsStore;
            this.commandQueue = commandQueue;
            this.commandQueue.OnError += CommandQueue_OnError;
            this.commandStore = commandStore;
            this.credentialsStore = credentialsStore;
            this.clientProvider = new NotifoClientProvider(httpClientFactory, credentialsStore);

            SetupPlatform();

            OnError += (sender, args) =>
            {
                Log.Error(args.Error, args.Exception);
            };
        }

        partial void SetupPlatform();

        /// <inheritdoc/>
        public void ClearAllSettings()
        {
            commandStore.Clear();
            credentialsStore.Clear();
            seenNotificationsStore.Clear();
        }

        /// <inheritdoc/>
        public INotifoMobilePush SetApiKey(string apiKey)
        {
            clientProvider.ApiKey = apiKey;
            return this;
        }

        /// <inheritdoc/>
        public INotifoMobilePush SetBaseUrl(string baseUrl)
        {
            clientProvider.ApiUrl = baseUrl;
            return this;
        }

        /// <inheritdoc/>
        public INotifoMobilePush SetApiVersion(ApiVersion apiVersion)
        {
            ApiVersion = apiVersion;
            return this;
        }

        /// <inheritdoc/>
        public HttpClient CreateHttpClient()
        {
            return clientProvider.CreateHttpClient();
        }

        /// <inheritdoc/>
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

            pushEventsProvider.GetTokenAsync().ContinueWith(tokenTask =>
            {
                if (tokenTask.Status == TaskStatus.RanToCompletion)
                {
                    UpdateToken(tokenTask.Result);
                }
            });

            return this;
        }

        /// <inheritdoc/>
        public void RaiseError(string error, Exception? exception, object? source)
        {
            OnError?.Invoke(this, new NotificationErrorEventArgs(error, exception, source));
        }

        /// <inheritdoc/>
        public void Register()
        {
            async Task RegisterAsync()
            {
                if (pushEventsProvider == null)
                {
                    return;
                }

                // Always fetch the token to avoid any consistency issues.
                var tokenToRegister = await pushEventsProvider.GetTokenAsync();

                Register(tokenToRegister);
            }

            _ = RegisterAsync();
        }

        /// <inheritdoc/>
        public void Register(string? tokenToRegister)
        {
            if (string.IsNullOrEmpty(tokenToRegister))
            {
                return;
            }

            commandQueue.Run(new TokenRegisterCommand { Token = tokenToRegister });
        }

        /// <inheritdoc/>
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

        private void UpdateToken(string? newToken)
        {
            if (!string.Equals(newToken, token))
            {
                token = newToken;

                // Only register the token if the client is already configured to avoid warning logs.
                if (!string.IsNullOrEmpty(clientProvider.ApiKey) &&
                    !string.IsNullOrEmpty(clientProvider.ApiUrl))
                {
                    Register(newToken);
                }
            }
        }
    }
}
