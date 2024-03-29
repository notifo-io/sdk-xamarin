﻿// ==========================================================================
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

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        public override void OnReceived(IDictionary<string, object> parameters)
        {
            NotifoIO.Current.RaiseDebug(Strings.ReceivedNotification, this, parameters);

            var notification = new UserNotificationDto().FromDictionary(parameters);

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
            var notification = new UserNotificationDto().FromDictionary(parameters);

            if (NotifoIO.Current is InternalAndroidPushAdapter adapter)
            {
                _ = adapter.OnBuildNotificationAsync(notificationBuilder, notification);
            }
        }
    }
}