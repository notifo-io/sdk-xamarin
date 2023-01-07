// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Android.App;
using Android.Content;
using Plugin.FirebasePushNotification;
using Notifo.SDK;
using System;
using System.Threading;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.FirebasePlugin;

/// <summary>
/// The <see cref="INotifoMobilePush"/> extension methods.
/// </summary>
public static partial class NotifoMobilePushExtensions
{
    private static bool isAndroidHandlerRegistered;

    /// <summary>
    /// Use the firebase plugin as the push events provider.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="context">The current application context.</param>
    /// <param name="resetToken">Set to <see langword="true"/> while debugging.</param>
    /// <param name="autoRegistration">Automatically register for push notifications.</param>
    /// <returns>The current instance.</returns>
    public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifo, Context context, bool resetToken = false, bool autoRegistration = true)
    {
        if (isAndroidHandlerRegistered)
        {
            return notifo;
        }

        FirebasePushNotificationManager.Initialize(context, new NotifoPushNotificationHandler(), resetToken, createDefaultNotificationChannel: true, autoRegistration);
        isAndroidHandlerRegistered = true;

        return notifo.UseFirebasePluginEventsProvider();
    }

    /// <summary>
    /// Method for processing open notification intent.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="activity">The current activity.</param>
    /// <param name="intent">The intent for processing.</param>
    public static void ProcessIntent(this INotifoMobilePush notifo, Activity activity, Intent intent)
    {
        notifo.UseFirebasePluginEventsProvider();
        FirebasePushNotificationManager.ProcessIntent(activity, intent, enableDelayedResponse: true);
    }
}
