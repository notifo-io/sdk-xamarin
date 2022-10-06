// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using AndroidX.Core.App;

namespace Notifo.SDK
{
    /// <summary>
    /// Notification handler interface.
    /// </summary>
    public interface INotificationHandler
    {
        /// <summary>
        /// Provides a place for notification customization.
        /// </summary>
        /// <param name="notificationBuilder">The notification builder.</param>
        /// <param name="notification">The notification.</param>
        void OnBuildNotification(NotificationCompat.Builder notificationBuilder, UserNotificationDto notification);
    }
}
