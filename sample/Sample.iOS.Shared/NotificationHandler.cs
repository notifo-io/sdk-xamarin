// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using System.Threading;
using System.Threading.Tasks;
using UserNotifications;

namespace Sample.iOS.Shared
{
    public class NotificationHandler : INotificationHandler
    {
        public Task OnBuildNotificationAsync(UNMutableNotificationContent content, UserNotificationDto notification,
            CancellationToken ct)
        {
            content.Sound = UNNotificationSound.GetSound("announcement.caf");

            return Task.CompletedTask;
        }
    }
}
