// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK.Resources;
using Serilog;
using UserNotifications;

namespace Notifo.SDK
{
    public static partial class NotifoIO
    {
        public static void DidReceiveNotificationRequest(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            Log.Debug(Strings.ReceivedNotification, request.Content.UserInfo);
        }
    }
}
