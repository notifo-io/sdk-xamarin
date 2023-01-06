// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using UserNotifications;

namespace Notifo.SDK;

/// <summary>
/// Notification handler interface.
/// </summary>
public interface INotificationHandler
{
    /// <summary>
    /// Provides a place for notification customization.
    /// </summary>
    /// <param name="content">The notification content.</param>
    /// <param name="notification">The notification.</param>
    void OnBuildNotification(UNMutableNotificationContent content, UserNotificationDto notification);
}
