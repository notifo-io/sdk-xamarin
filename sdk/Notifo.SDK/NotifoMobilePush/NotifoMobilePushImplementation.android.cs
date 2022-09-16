// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.Core.App;
using Java.Net;
using Microsoft.Extensions.Caching.Memory;
using Notifo.SDK.Resources;
using Serilog;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation
    {
        private readonly IMemoryCache bitmapCache = new MemoryCache(new MemoryCacheOptions());
        private INotificationHandler? notificationHandler;

        partial void SetupPlatform()
        {
            OnNotificationReceived += PushEventsProvider_OnNotificationReceivedAndroid;
        }

        private void PushEventsProvider_OnNotificationReceivedAndroid(object sender, NotificationEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Notification.TrackSeenUrl))
            {
                _ = TrackNotificationsAsync(e.Notification);
            }
        }

        public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler)
        {
            this.notificationHandler = notificationHandler;

            return this;
        }

        internal void OnBuildNotification(NotificationCompat.Builder notificationBuilder, UserNotificationDto notification)
        {
            if (!string.IsNullOrWhiteSpace(notification.Subject))
            {
                notificationBuilder.SetContentTitle(notification.Subject);
            }

            if (!string.IsNullOrWhiteSpace(notification.ImageSmall))
            {
                var smallWidth = GetDimension(Resource.Dimension.notification_large_icon_width);
                var smallHeight = GetDimension(Resource.Dimension.notification_large_icon_height);

                var largeIcon = GetBitmap(notification.ImageSmall, smallWidth, smallHeight);
                if (largeIcon != null)
                {
                    notificationBuilder.SetLargeIcon(largeIcon);
                }
            }

            if (!string.IsNullOrWhiteSpace(notification.ImageLarge))
            {
                var bigPicture = GetBitmap(notification.ImageLarge);
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

            notificationHandler?.OnBuildNotification(notificationBuilder, notification);
        }

        private Bitmap? GetBitmap(string bitmapUrl, int requestWidth = -1, int requestHeight = -1)
        {
            try
            {
                if (requestWidth > 0 && requestHeight > 0)
                {
                    bitmapUrl = $"{bitmapUrl}?width={requestWidth}&height={requestHeight}";
                }

                if (bitmapCache.TryGetValue(bitmapUrl, out Bitmap cachedBitmap))
                {
                    return cachedBitmap;
                }

                var bitmapStream = new URL(bitmapUrl)?.OpenConnection()?.InputStream;

                var bitmap = BitmapFactory.DecodeStream(bitmapStream);
                if (bitmap != null)
                {
                    bitmapCache.Set(bitmapUrl, bitmap);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Log.Error(Strings.DownloadImageError, ex);
            }

            return null;
        }

        private void AddAction(NotificationCompat.Builder notificationBuilder, string title, string url)
        {
            var notificationIntent = new Intent(Intent.ActionView);

            // Set the URL to open when the button is clicked.
            notificationIntent.SetData(Android.Net.Uri.Parse(url));

            var buttonIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent,
                PendingIntentFlags.UpdateCurrent |
                PendingIntentFlags.Immutable);

            notificationBuilder.AddAction(0, title, buttonIntent);
        }

        private int GetDimension(int resourceId)
        {
            return Application.Context?.Resources?.GetDimensionPixelSize(resourceId) ?? -1;
        }
    }
}
