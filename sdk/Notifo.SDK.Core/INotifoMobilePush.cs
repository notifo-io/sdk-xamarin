﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK
{
    /// <summary>
    /// Notifo mobile push service interface.
    /// </summary>
    public partial interface INotifoMobilePush
    {
        /// <summary>
        /// Event triggered when a notification is received.
        /// </summary>
        event EventHandler<NotificationEventArgs> OnNotificationReceived;

        /// <summary>
        /// Event triggered when a notification is opened.
        /// </summary>
        event EventHandler<NotificationEventArgs> OnNotificationOpened;

        /// <summary>
        /// Event triggered when an log event happened.
        /// </summary>
        event EventHandler<NotificationLogEventArgs> OnLog;

        /// <summary>
        /// Gets the notifo client.
        /// </summary>
        INotifoClient Client { get; }

        /// <summary>
        /// The used API version.
        /// </summary>
        ApiVersion ApiVersion { get; }

        /// <summary>
        /// Indicates whether the client is configured.
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Returns identifier of the device.
        /// </summary>
        string DeviceIdentifier { get; }

        /// <summary>
        /// Clears all settings that are currently stored.
        /// </summary>
        /// <returns>The current instance.</returns>
        INotifoMobilePush ClearAllSettings();

        /// <summary>
        /// Sets the shared name to store settings.
        /// </summary>
        /// <param name="sharedName">The shared name.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetSharedName(string sharedName);

        /// <summary>
        /// Sets the API key to use.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetApiKey(string apiKey);

        /// <summary>
        /// Sets the API version to use.
        /// </summary>
        /// <param name="apiVersion">The API version.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetApiVersion(ApiVersion apiVersion);

        /// <summary>
        /// Sets the api URL to use.
        /// </summary>
        /// <param name="baseUrl">The api URL. Default: https://app.notifo.io.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetBaseUrl(string baseUrl);

        /// <summary>
        /// Sets the push events provider.
        /// </summary>
        /// <param name="pushEventsProvider">The <see cref="IPushEventsProvider"/> implementation. </param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);

        /// <summary>
        /// Raises an error.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source of the error.</param>
        void RaiseError(string error, Exception? exception, object? source);

        /// <summary>
        /// Raises a debug message.
        /// </summary>
        /// <param name="message">The debug message.</param>
        /// <param name="source">The source of the error.</param>
        /// <param name="args">The message arguments.</param>
        void RaiseDebug(string message, object? source, params object[] args);

        /// <summary>
        /// Register for notifications on demand.
        /// </summary>
        void Register();

        /// <summary>
        /// Register for notifications on demand.
        /// </summary>
        /// <param name="tokenToRegister">The token to register.</param>
        void Register(string tokenToRegister);

        /// <summary>
        /// Unregister notifications on demand.
        /// </summary>
        void Unregister();

        /// <summary>
        /// Waits for all background tasks to be completed.
        /// </summary>
        /// <param name="ct">The cancellation token to abort waiting.</param>
        /// <returns>The tasks.</returns>
        Task WaitForBackgroundTasksAsync(
            CancellationToken ct = default(CancellationToken));
    }
}