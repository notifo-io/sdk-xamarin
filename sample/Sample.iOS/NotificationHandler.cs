// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using UserNotifications;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace Sample.iOS
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public class NotificationHandler : INotificationHandler
    {
        public void OnBuildNotification(UNMutableNotificationContent content, NotificationDto notification)
        {
            content.Sound = UNNotificationSound.GetSound("announcement.caf");
        }
    }
}