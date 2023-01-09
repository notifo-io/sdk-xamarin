// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using UserNotifications;

namespace Notifo.SDK;

public partial interface INotifoMobilePush
{
    /// <summary>
    /// Sets the notification handler.
    /// </summary>
    /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
    /// <returns>The current instance.</returns>
    INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler);

    /// <summary>
    /// Method for processing notification before delivery.
    /// </summary>
    /// <param name="request">The request that was received.</param>
    /// <param name="content">The notification content to enrich.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent content);

    /// <summary>
    /// Method for pulling pending notifications.
    /// </summary>
    /// <param name="options">The options for handling the pending notifications pull refresh request.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task DidReceivePullRefreshRequestAsync(PullRefreshOptions? options = null);

    /// <summary>
    /// Method for processing the user's response to a delivered notification.
    /// </summary>
    /// <param name="response">The user's response to the notification.</param>
    void DidReceiveNotificationResponse(UNNotificationResponse response);
}
