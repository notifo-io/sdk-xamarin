// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
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

            if (parameters.TryGetValue(IdKey, out var id))
            {
                var notificationId = Math.Abs(id.GetHashCode());
                parameters[IdKey] = notificationId;
            }

            base.OnReceived(parameters);
        }

        public override void OnBuildNotification(NotificationCompat.Builder notificationBuilder, IDictionary<string, object> parameters)
        {
            var notification = new NotificationDto()
                .FromDictionary(new Dictionary<string, object>(parameters));

            if (!string.IsNullOrWhiteSpace(notification.Subject))
            {
                notificationBuilder.SetContentTitle(notification.Subject);
            }

            if (!string.IsNullOrWhiteSpace(notification.ImageSmall))
            {
                int width = GetDimension(Resource.Dimension.notification_large_icon_width);
                int height = GetDimension(Resource.Dimension.notification_large_icon_height);

                var largeIcon = notifoMobilePush.GetBitmap(notification.ImageSmall, width, height);
                if (largeIcon != null)
                {
                    notificationBuilder.SetLargeIcon(largeIcon);
                }
            }

            if (!string.IsNullOrWhiteSpace(notification.ImageLarge))
            {
                var bigPicture = notifoMobilePush.GetBitmap(notification.ImageLarge);
                if (bigPicture != null)
                {
                    notificationBuilder.SetStyle(
                        new NotificationCompat
                            .BigPictureStyle()
                            .BigPicture(bigPicture)
                            .SetSummaryText(notification.Body)
                    );
                }
            }

            notificationBuilder.MActions.Clear();

            if (!string.IsNullOrWhiteSpace(notification.ConfirmUrl) &&
                !string.IsNullOrWhiteSpace(notification.ConfirmText) &&
                !notification.IsConfirmed)
            {
                AddAction(notificationBuilder, notification.ConfirmText, notification.ConfirmUrl);
            }

            if (!string.IsNullOrWhiteSpace(notification.LinkUrl) &&
                !string.IsNullOrWhiteSpace(notification.LinkText))
            {
                AddAction(notificationBuilder, notification.LinkText, notification.LinkUrl);
            }
        }

        private void AddAction(NotificationCompat.Builder notificationBuilder, string title, string url)
        {
            var notificationIntent = new Intent(Intent.ActionView);
            notificationIntent.SetData(Android.Net.Uri.Parse(url));
            var buttonIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, 0);

            notificationBuilder.AddAction(0, title, buttonIntent);
        }

        private int GetDimension(int resourceId) =>
            Application.Context?.Resources?.GetDimensionPixelSize(resourceId) ?? -1;
    }
}
