// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.NotifoMobilePush
{
    public interface INotifoMobilePush : INotifoClient
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
        /// Sets the api key to use.
        /// </summary>
        /// <param name="apiKey">
        /// The api key.
        /// </param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetApiKey(string apiKey);

        /// <summary>
        /// Sets the api URL to use.
        /// </summary>
        /// <param name="baseUrl">
        /// The api URL. Default: https://app.notifo.io.
        /// </param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetBaseUrl(string baseUrl);

        /// <summary>
        /// Sets the push events provider.
        /// </summary>
        /// <param name="pushEventsProvider">
        /// The <see cref="IPushEventsProvider"/> implementation.
        /// </param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);

        /// <summary>
        /// Register for notifications on demand.
        /// </summary>
        void Register();
    }
}
