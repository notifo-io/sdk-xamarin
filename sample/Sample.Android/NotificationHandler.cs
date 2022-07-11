// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Notifo.SDK;

namespace Sample.Droid
{
    public class NotificationHandler : INotificationHandler
    {
        public void OnBuildNotification(NotificationCompat.Builder notificationBuilder, NotificationDto notification)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                return;
            }

            var soundUri = Android.Net.Uri.Parse($"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/announcement");

            notificationBuilder.SetSound(soundUri);
        }
    }
}