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
            if (parameters.TryGetValue(Constants.SubjectKey, out var subject))
            {
                notificationBuilder.SetContentTitle(subject?.ToString());
            }

            if (parameters.TryGetValue(Constants.ImageSmallKey, out var largeIconUrl))
            {
                int width = GetDimension(Resource.Dimension.notification_large_icon_width);
                int height = GetDimension(Resource.Dimension.notification_large_icon_height);

                var largeIcon = notifoMobilePush.GetBitmap(largeIconUrl.ToString(), width, height);
                if (largeIcon != null)
                {
                    notificationBuilder.SetLargeIcon(largeIcon);
                }
            }

            if (parameters.TryGetValue(Constants.ImageLargeKey, out var bigPictureUrl))
            {
                var bigPicture = notifoMobilePush.GetBitmap(bigPictureUrl.ToString());
                if (bigPicture != null)
                {
                    parameters.TryGetValue(BodyKey, out var summaryText);

                    notificationBuilder.SetStyle(
                        new NotificationCompat
                            .BigPictureStyle()
                            .BigPicture(bigPicture)
                            .SetSummaryText(summaryText?.ToString())
                    );
                }
            }

            if (parameters.TryGetValue(Constants.ConfirmUrlKey, out var confirmUrl) &&
                parameters.TryGetValue(Constants.ConfirmTextKey, out var confirmText))
            {
                AddAction(notificationBuilder, confirmText.ToString(), confirmUrl.ToString());
            }

            if (parameters.TryGetValue(Constants.LinkUrlKey, out var linkUrl) &&
                parameters.TryGetValue(Constants.LinkTextKey, out var linkText))
            {
                AddAction(notificationBuilder, linkText.ToString(), linkUrl.ToString());
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
