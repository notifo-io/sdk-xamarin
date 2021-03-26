// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading;
using Android.Support.V4.App;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        private const string SubjectKey = "subject";
        private static int notificationId;

        public override void OnReceived(IDictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(IdKey))
            {
                Interlocked.Increment(ref notificationId);
                parameters[IdKey] = notificationId;
            }

            base.OnReceived(parameters);
        }

        public override void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(SubjectKey, out var subject))
            {
                notificationBuilder.SetContentTitle(subject?.ToString());
            }
        }
    }
}
