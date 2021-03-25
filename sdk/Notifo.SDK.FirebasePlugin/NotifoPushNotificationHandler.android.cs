// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Android.Support.V4.App;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        private const string SubjectKey = "subject";

        public override void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(SubjectKey, out var subject))
            {
                notificationBuilder.SetContentTitle(subject?.ToString());
            }
        }
    }
}
