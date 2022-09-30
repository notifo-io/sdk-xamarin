// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using UserNotifications;

namespace Sample.iOS.Shared
{
    public class NotificationHandler : INotificationHandler
    {
        public void OnBuildNotification(UNMutableNotificationContent content, UserNotificationDto notification)
        {
            content.Sound = UNNotificationSound.GetSound("announcement.caf");
        }
    }
}
