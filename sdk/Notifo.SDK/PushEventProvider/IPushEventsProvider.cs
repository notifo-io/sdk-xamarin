// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;

namespace Notifo.SDK.PushEventProvider;

/// <summary>
/// Push events provider interface.
/// </summary>
public interface IPushEventsProvider
{
    /// <summary>
    /// Event triggered when token is refreshed.
    /// </summary>
    event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

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
    /// Gets the current token.
    /// </summary>
    /// <returns>The token.</returns>
    public Task<string?> GetTokenAsync();
}
