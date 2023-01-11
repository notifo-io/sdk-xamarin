// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Extensions;
using Notifo.SDK.Helpers;
using Notifo.SDK.PushEventProvider;
using Notifo.SDK.Resources;

namespace Notifo.SDK.NotifoMobilePush;

internal partial class NotifoMobilePushImplementation : INotifoMobilePush
{
    private const int Capacity = 500;
    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
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
    public event EventHandler<NotificationLogEventArgs> OnLog;

    /// <inheritdoc/>
    public ApiVersion ApiVersion { get; private set; }

    /// <inheritdoc/>
    public bool IsConfigured => options.IsConfigured;

    /// <inheritdoc/>
    public string DeviceIdentifier => credentialsStore.DeviceIdentifier;

    public NotifoMobilePushImplementation(
        ISeenNotificationsStore seenNotificationsStore,
        ICommandQueue commandQueue,
        ICommandStore commandStore,
        ICredentialsStore credentialsStore)
    {
        this.options = new NotifoOptions(credentialsStore);
        this.seenNotificationsStore = seenNotificationsStore;
        this.commandQueue = commandQueue;
        this.commandQueue.OnLog += CommandQueue_OnError;
        this.commandStore = commandStore;
        this.credentialsStore = credentialsStore;
        this.Client = NotifoClientBuilder.Create().SetOptions(options).Build();

        SetupPlatform();
    }

    partial void SetupPlatform();

    /// <inheritdoc/>
    public INotifoMobilePush ClearAllSettings()
    {
        static void Clear(object store)
        {
            if (store is IClearableStore clearable)
            {
                clearable.Clear();
            }
        }

        Clear(commandStore);
        Clear(credentialsStore);
        Clear(seenNotificationsStore);
        return this;
    }

    /// <inheritdoc/>
    public INotifoMobilePush SetSharedName(string sharedName)
    {
        static void Configure(object store, string sharedName)
        {
            if (store is ISettingsStore settings)
            {
                settings.SharedName = sharedName;
            }
        }

        Configure(commandStore, sharedName);
        Configure(credentialsStore, sharedName);
        Configure(seenNotificationsStore, sharedName);
        return this;
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
            this.pushEventsProvider.OnLog -= PushEventsProvider_OnLog;
        }

        this.pushEventsProvider = pushEventsProvider;

        if (this.pushEventsProvider != null)
        {
            this.pushEventsProvider.OnTokenRefresh += PushEventsProvider_OnTokenRefresh;
            this.pushEventsProvider.OnNotificationReceived += PushEventsProvider_OnNotificationReceived;
            this.pushEventsProvider.OnNotificationOpened += PushEventsProvider_OnNotificationOpened;
            this.pushEventsProvider.OnLog += PushEventsProvider_OnLog;
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
        OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Error, source, error, null, exception));
    }

    /// <inheritdoc/>
    public void RaiseDebug(string message, object? source, params object[] args)
    {
        OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Debug, source, message, args, null));
    }

    /// <inheritdoc/>
    public Task WaitForBackgroundTasksAsync(
        CancellationToken ct = default)
    {
        return commandQueue.CompleteAsync(ct);
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

        RegisterAsync().Forget();
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

    private void PushEventsProvider_OnLog(object sender, NotificationLogEventArgs e)
    {
        // Forward the event to the application.
        OnLog?.Invoke(sender, e);
    }

    private void CommandQueue_OnError(object sender, NotificationLogEventArgs e)
    {
        // Forward the event to the application.
        OnLog?.Invoke(sender, e);
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

    public async Task<HashSet<Guid>> GetSeenNotificationsAsync()
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            // Always load the notifications from the preferences, because they could have been modified by the service extension.
            var loaded = await seenNotificationsStore.GetSeenNotificationIdsAsync();

            return loaded.ToHashSet();
        }
        catch (Exception ex)
        {
            NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
            return new HashSet<Guid>();
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task TrackNotificationsAsync(params UserNotificationDto[] notifications)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var idsAndUrls = notifications.ToDictionary(x => x.Id, x => (string?)x.TrackSeenUrl);

            // Always load the notifications from the preferences, because they could have been modified by the service extension.
            var loaded = await seenNotificationsStore.GetSeenNotificationIdsAsync();

            // Store the seen notifications immediately as a cache, if the actual command to the server fails.
            await seenNotificationsStore.AddSeenNotificationIdsAsync(Capacity, idsAndUrls.Keys);

            foreach (var id in idsAndUrls.Keys)
            {
                loaded.Add(id, Capacity);
            }

            // Track all notifications with one HTTP request.
            commandQueue.Run(new TrackSeenCommand { IdsAndUrls = idsAndUrls, Token = token });
        }
        catch (Exception ex)
        {
            NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
            return;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
}
