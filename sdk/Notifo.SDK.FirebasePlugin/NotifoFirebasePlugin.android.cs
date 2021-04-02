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

namespace Notifo.SDK.FirebasePlugin
{
    public class NotifoFirebasePlugin
    {
        public static void Initialize(Context context, INotifoStartup notifoStartup, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, new NotifoPushNotificationHandler(), resetToken, createDefaultNotificationChannel, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static void Initialize(Context context, INotifoStartup notifoStartup, NotificationUserCategory[] notificationCategories, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, notificationCategories, resetToken, createDefaultNotificationChannel, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static void Initialize(Context context, INotifoStartup notifoStartup, IPushNotificationHandler pushNotificationHandler, bool resetToken, bool createDefaultNotificationChannel = true, bool autoRegistration = true)
        {
            ConfigureDefaultChannel();

            FirebasePushNotificationManager.Initialize(context, pushNotificationHandler, resetToken, createDefaultNotificationChannel, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static void ProcessIntent(Activity activity, Intent intent, bool enableDelayedResponse = true)
        {
            FirebasePushNotificationManager.ProcessIntent(activity, intent, enableDelayedResponse);
            NotifoIO.Current.UseFirebasePluginEventsProvider();
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
