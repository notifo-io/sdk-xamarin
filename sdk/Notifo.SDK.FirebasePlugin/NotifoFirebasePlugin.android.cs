// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Android.App;
using Android.Content;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    /// <summary>
    /// Plugin initialization.
    /// </summary>
    public class NotifoFirebasePlugin
    {
        /// <summary>
        /// Initializes the firebase plugin.
        /// </summary>
        /// <param name="context">The current application context.</param>
        /// <param name="notifoStartup">The <see cref="INotifoStartup"/> implementation.</param>
        /// <param name="resetToken">Set to <see langword="true"/> while debugging.</param>
        /// <param name="autoRegistration">Automatically register for push notifications.</param>
        public static void Initialize(Context context, INotifoStartup notifoStartup, bool resetToken, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(context, new NotifoPushNotificationHandler(), resetToken, createDefaultNotificationChannel: true, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        /// <summary>
        /// Method for processing open notification intent.
        /// </summary>
        /// <param name="activity">The current activity.</param>
        /// <param name="intent">The intent for processing.</param>
        public static void ProcessIntent(Activity activity, Intent intent)
        {
            FirebasePushNotificationManager.ProcessIntent(activity, intent, enableDelayedResponse: true);
            NotifoIO.Current.UseFirebasePluginEventsProvider();
        }
    }
}
