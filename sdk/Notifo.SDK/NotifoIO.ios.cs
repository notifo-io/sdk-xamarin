// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Notifo.SDK.Resources;
using Serilog;
using UserNotifications;

namespace Notifo.SDK
{
    public static partial class NotifoIO
    {
        public static async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            Log.Debug(Strings.ReceivedNotification, request.Content.UserInfo);

            if (Current is NotifoMobilePush notifoMobilePush)
            {
                await notifoMobilePush.DidReceiveNotificationRequestAsync(request, bestAttemptContent);
            }
        }

        public static async Task DidReceivePullRefreshNotificationAsync()
        {
            if (Current is NotifoMobilePush notifoMobilePush)
            {
                await notifoMobilePush.DidReceivePullRefreshNotificationAsync();
            }
        }
    }
}
