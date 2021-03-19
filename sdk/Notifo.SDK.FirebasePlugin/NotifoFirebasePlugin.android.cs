// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Android.App;
using Android.Content;
using Android.OS;
using Plugin.FirebasePushNotification;

namespace NotifoIO.SDK.FirebasePlugin
{
    public class NotifoFirebasePlugin
    {
        public static void Initialize(Context context, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, resetToken, createDefaultNotificationChannel, autoRegistration);
            Notifo.Current.UseFirebasePluginEventsProvider();
        }

        public static void Initialize(Context context, NotificationUserCategory[] notificationCategories, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, notificationCategories, resetToken, createDefaultNotificationChannel, autoRegistration);
            Notifo.Current.UseFirebasePluginEventsProvider();
        }

        public static void Initialize(Context context, IPushNotificationHandler pushNotificationHandler, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, pushNotificationHandler, resetToken, createDefaultNotificationChannel, autoRegistration);
            Notifo.Current.UseFirebasePluginEventsProvider();
        }

        public static void ProcessIntent(Activity activity, Intent intent, bool enableDelayedResponse = true)
        {
            FirebasePushNotificationManager.ProcessIntent(activity, intent, enableDelayedResponse);
            Notifo.Current.UseFirebasePluginEventsProvider();
        }

        private static void ConfigureDefaultChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }
        }
    }
}
