// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Notifo.SDK;

namespace Sample.Droid
{
    public class NotificationHandler : INotificationHandler
    {
        public Task OnBuildNotificationAsync(NotificationCompat.Builder notificationBuilder, UserNotificationDto notification,
            CancellationToken ct)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                return Task.CompletedTask;
            }

            var soundUri = Android.Net.Uri.Parse($"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/announcement");

            notificationBuilder.SetSound(soundUri);
            return Task.CompletedTask;
        }
    }
}