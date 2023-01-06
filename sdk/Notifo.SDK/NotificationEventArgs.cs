// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK;

/// <summary>
/// Event arguments containing notification data.
/// </summary>
public class NotificationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the notification.
    /// </summary>
    public UserNotificationDto Notification { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class with the notification token.
    /// </summary>
    /// <param name="notification">The push notification.</param>
    public NotificationEventArgs(UserNotificationDto notification)
    {
        Notification = notification;
    }
}
