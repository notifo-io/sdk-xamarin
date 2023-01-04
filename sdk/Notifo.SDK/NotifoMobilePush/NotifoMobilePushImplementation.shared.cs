// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Helpers;
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
        private readonly NotifoOptions options;
        private IPushEventsProvider? pushEventsProvider;
        private string? token;

        /// <inheritdoc/>
        public INotifoClient Client { get; }

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> OnNotificationOpened;

        /// <inheritdoc/>
        public event EventHandler<NotificationErrorEventArgs> OnError;

        /// <inheritdoc/>
        public ApiVersion ApiVersion { get; private set; }

        /// <inheritdoc/>
        public bool IsConfigured => options.IsConfigured;

        public NotifoMobilePushImplementation(
            ISeenNotificationsStore seenNotificationsStore,
            ICommandQueue commandQueue,
            ICommandStore commandStore,
            ICredentialsStore credentialsStore)
        {
            this.options = new NotifoOptions(credentialsStore);
            this.seenNotificationsStore = seenNotificationsStore;
            this.commandQueue = commandQueue;
            this.commandQueue.OnError += CommandQueue_OnError;
            this.commandStore = commandStore;
            this.credentialsStore = credentialsStore;
            this.Client = NotifoClientBuilder.Create().SetHttpClientProvider(new NotifoHttpClientProvider(options)).Build();

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
            options.ApiKey = apiKey;
            return this;
        }

        /// <inheritdoc/>
        public INotifoMobilePush SetBaseUrl(string baseUrl)
        {
            options.ApiUrl = baseUrl;
            return this;
        }

        /// <inheritdoc/>
        public INotifoMobilePush SetApiVersion(ApiVersion apiVersion)
        {
            ApiVersion = apiVersion;
            return this;
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

            commandQueue.Run(new TokenRegisterCommand { Token = tokenToRegister! });
        }

        /// <inheritdoc/>
        public void Unregister()
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            commandQueue.Run(new TokenUnregisterCommand { Token = token! });
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
                if (IsConfigured)
                {
                    Register(newToken);
                }
            }
        }
    }
}
