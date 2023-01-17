// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.Core.App;
using Java.Net;
using Notifo.SDK.Extensions;
using Notifo.SDK.Helpers;
using Notifo.SDK.Resources;

namespace Notifo.SDK.NotifoMobilePush;

internal partial class NotifoMobilePushImplementation : InternalAndroidPushAdapter
{
    private readonly LRUCache<string, Bitmap> bitmapCache = new LRUCache<string, Bitmap>(10_000_000);
    private INotificationHandler? notificationHandler;

    partial void SetupPlatform()
    {
        OnNotificationReceived += PushEventsProvider_OnNotificationReceivedAndroid;
    }

    /// <inheritdoc />
    public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler)
    {
        this.notificationHandler = notificationHandler;
        return this;
    }

    /// <inheritdoc />
    public INotifoMobilePush SetImageCacheCapacity(int capacity)
    {
        bitmapCache.EnsureCapacity(capacity);
        return this;
    }

    private void PushEventsProvider_OnNotificationReceivedAndroid(object sender, NotificationEventArgs e)
    {
        TrackNotificationsAsync(e.Notification).Forget();
    }

    /// <inheritdoc />
    public async Task OnBuildNotificationAsync(NotificationCompat.Builder notificationBuilder, UserNotificationDto notification)
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
                        .SetSummaryText(notification.Body));
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

        if (notificationHandler != null)
        {
            await notificationHandler.OnBuildNotificationAsync(notificationBuilder, notification, default);
        }
    }

    private Bitmap? GetBitmap(string bitmapUrl, int? width = null, int? height = null)
    {
        try
        {
            // Let the server resize the image to the perfect format.
            if (width != null && height != null)
            {
                bitmapUrl = bitmapUrl.AppendQueries("width", width, "height", height);
            }

            if (bitmapCache.TryGetValue(bitmapUrl, out var cachedBitmap))
            {
                return cachedBitmap;
            }

            var bitmapStream = new URL(bitmapUrl)?.OpenConnection()?.InputStream;
            var bitmapImage = BitmapFactory.DecodeStream(bitmapStream);

            if (bitmapImage != null)
            {
                bitmapCache.Set(bitmapUrl, bitmapImage, bitmapImage.ByteCount);
            }
            else
            {
                NotifoIO.Current.RaiseError(Strings.DecodingImageError, null, this);
            }

            return bitmapImage;
        }
        catch (Exception ex)
        {
            NotifoIO.Current.RaiseError(Strings.DownloadImageError, ex, this);
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
