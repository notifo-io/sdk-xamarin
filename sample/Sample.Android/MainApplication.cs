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
using Notifo.SDK.FirebasePlugin;
using Plugin.FirebasePushNotification;

namespace Sample.Droid
{
    [Application]
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

#if DEBUG
            NotifoFirebasePlugin.Initialize(this, new NotifoStartup(), resetToken: true);
#else
            NotifoFirebasePlugin.Initialize(this, new NotifoStartup(), resetToken: false);
#endif
        }
    }
}