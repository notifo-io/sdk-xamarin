// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Notifo.SDK.NotifoMobilePush;
using UserNotifications;

namespace Notifo.SDK
{
    public static partial class NotifoIO
    {
        public static async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                await notifoMobilePush.DidReceiveNotificationRequestAsync(request, bestAttemptContent);
            }
        }

        public static async Task DidReceivePullRefreshRequestAsync()
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                await notifoMobilePush.DidReceivePullRefreshRequestAsync();
            }
        }

        public static void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                notifoMobilePush.DidReceiveNotificationResponse(center, response, completionHandler);
            }
        }
    }
}
