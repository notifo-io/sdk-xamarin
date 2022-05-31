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
        private INotificationHandler? notificationHandler;
        public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler)
        {
            this.notificationHandler = notificationHandler;

            return this;
        }

        private readonly IMemoryCache bitmapCache = new MemoryCache(new MemoryCacheOptions());

        internal void OnBuildNotification(NotificationCompat.Builder notificationBuilder, NotificationDto notification)
        {
            if (!string.IsNullOrWhiteSpace(notification.Subject))
            {
                notificationBuilder.SetContentTitle(notification.Subject);
            }

            if (!string.IsNullOrWhiteSpace(notification.ImageSmall))
            {
                int width = GetDimension(Resource.Dimension.notification_large_icon_width);
                int height = GetDimension(Resource.Dimension.notification_large_icon_height);

                var largeIcon = GetBitmap(notification.ImageSmall, width, height);
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
                bool shouldResize = requestWidth > 0 && requestHeight > 0;
                bool resizeHandled = false;

                if (shouldResize && bitmapUrl.StartsWith(clientProvider.ApiUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    bitmapUrl = $"{bitmapUrl}?width={requestWidth}&height={requestHeight}";
                    resizeHandled = true;
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

                if (!resizeHandled && shouldResize)
                {
                    bitmap = ResizeBitmap(bitmap, requestWidth, requestHeight);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Log.Error(Strings.DownloadImageError, ex);
            }

            return null;
        }

        private Bitmap? ResizeBitmap(Bitmap? bitmap, int requestWidth, int requestHeight)
        {
            if (bitmap == null)
            {
                return null;
            }

            try
            {
                if (bitmap.Width > requestWidth || bitmap.Height > requestHeight)
                {
                    int newWidth = requestWidth;
                    int newHeight = requestHeight;

                    if (bitmap.Height > bitmap.Width)
                    {
                        float ratio = (float)bitmap.Width / bitmap.Height;
                        newWidth = (int)(newHeight * ratio);
                    }
                    else if (bitmap.Width > bitmap.Height)
                    {
                        float ratio = (float)bitmap.Height / bitmap.Width;
                        newHeight = (int)(newWidth * ratio);
                    }

                    return Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Strings.ResizeImageError, ex);
            }

            return bitmap;
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
