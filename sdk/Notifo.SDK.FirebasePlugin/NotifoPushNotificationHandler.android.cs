// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using Serilog;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        private const string SubjectKey = "subject";

        public override void OnReceived(IDictionary<string, object> parameters)
        {
            Log.Debug(Strings.ReceivedNotification, parameters);

            if (parameters.TryGetValue(IdKey, out var id))
            {
                var notificationId = Math.Abs(id.GetHashCode());
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
