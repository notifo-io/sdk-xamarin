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
using Android.Graphics;
using Android.Support.V4.App;
using Java.Net;
using Microsoft.Extensions.Caching.Memory;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using Serilog;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        private IMemoryCache bitmapCache;

        public NotifoPushNotificationHandler()
        {
            bitmapCache = new MemoryCache(new MemoryCacheOptions());
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

                var largeIcon = GetBitmap(largeIconUrl.ToString(), width, height);
                if (largeIcon != null)
                {
                    notificationBuilder.SetLargeIcon(largeIcon);
                }
            }

            if (parameters.TryGetValue(Constants.ImageLargeKey, out var bigPictureUrl))
            {
                var bigPicture = GetBitmap(bigPictureUrl.ToString());
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

            if (parameters.TryGetValue(Constants.ConfirmUrlKey, out var confirmUrl))
            {
                parameters.TryGetValue(Constants.ConfirmTextKey, out var confirmText);

                var notificationIntent = new Intent(Intent.ActionView);
                notificationIntent.SetData(Android.Net.Uri.Parse(confirmUrl.ToString()));
                var buttonIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, 0);

                notificationBuilder.AddAction(0, confirmText?.ToString(), buttonIntent);
            }
        }

        private int GetDimension(int resourceId) =>
            Application.Context?.Resources?.GetDimensionPixelSize(resourceId) ?? -1;

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

                var inputStream = new URL(bitmapUrl)?.OpenConnection()?.InputStream;

                var bitmap = BitmapFactory.DecodeStream(inputStream);
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
    }
}
