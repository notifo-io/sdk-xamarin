// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Notifo.SDK;
using Notifo.SDK.FirebasePlugin;
using Plugin.FirebasePushNotification;

namespace Sample.Droid
{
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }

            var resetToken = false;
#if DEBUG
            resetToken = true;
#endif
            NotifoIO.Current
                .SetNotificationHandler(new NotificationHandler())
                .UseDefaults()
                .UseFirebasePluginEventsProvider(this, resetToken: resetToken);
        }
    }
}