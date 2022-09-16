// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK.PushEventProvider
{
    /// <summary>
    /// Event arguments containing notification data.
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the notification.
        /// </summary>
        public NotificationDto Notification { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class with the notification token.
        /// </summary>
        /// <param name="notification">The push notification.</param>
        public NotificationEventArgs(NotificationDto notification)
        {
            Notification = notification;
        }
    }
}
