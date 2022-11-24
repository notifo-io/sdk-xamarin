// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using AndroidX.Core.App;
using Notifo.SDK.Extensions;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using Serilog;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        private NotifoMobilePushImplementation notifoMobilePush;

        public NotifoPushNotificationHandler()
        {
            notifoMobilePush = (NotifoMobilePushImplementation)NotifoIO.Current;
        }

        public override void OnReceived(IDictionary<string, object> parameters)
        {
            Log.Debug(Strings.ReceivedNotification, parameters);

            var notification = new UserNotificationDto()
                .FromDictionary(new Dictionary<string, object>(parameters));

            if (notification.Silent)
            {
                return;
            }

            if (parameters.TryGetValue(IdKey, out var id))
            {
                var notificationId = Math.Abs(id.GetHashCode());

                parameters[IdKey] = notificationId;
            }

            base.OnReceived(parameters);
        }

        public override void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            var notification = new UserNotificationDto()
                .FromDictionary(new Dictionary<string, object>(parameters));

            notifoMobilePush.OnBuildNotification(notificationBuilder, notification);
        }
    }
}
