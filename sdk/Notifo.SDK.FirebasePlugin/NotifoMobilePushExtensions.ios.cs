// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.PushEventProvider;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using UserNotifications;

namespace Notifo.SDK.FirebasePlugin;

/// <summary>
/// The <see cref="INotifoMobilePush"/> extension methods.
/// </summary>
public static partial class NotifoMobilePushExtensions
{
    private static bool isIosHandlerRegistered;

    /// <summary>
    /// Use the firebase plugin as the push events provider.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="options">Application launch options.</param>
    /// <param name="autoRegistration">Automatically register for push notifications.</param>
    /// <returns>The current instance.</returns>
    public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifo, NSDictionary options, bool autoRegistration = true)
    {
        if (isIosHandlerRegistered)
        {
            return notifo;
        }

        isIosHandlerRegistered = true;
        FirebasePushNotificationManager.Initialize(options, autoRegistration);

        return notifo.SetPushEventsProvider(new PluginEventsProvider());
    }

    /// <summary>
    /// Invoked when the application received a remote notification.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="data">The notification data dictionary.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static async Task DidReceiveMessageAsync(this INotifoMobilePush notifo, NSDictionary data)
    {
        notifo.RaiseDebug(Strings.ReceivedNotification, null, data);

        static bool ContainsPullRefreshRequest(NSDictionary data)
        {
            var aps = data?.ObjectForKey(new NSString(Constants.ApsKey)) as NSDictionary;

            return aps?.ContainsKey(new NSString(Constants.ContentAvailableKey)) == true;
        }

        if (ContainsPullRefreshRequest(data) && notifo is InternalIOSPushAdapter adapter)
        {
            await adapter.DidReceivePullRefreshRequestAsync();
        }

        FirebasePushNotificationManager.DidReceiveMessage(data);
    }

    /// <summary>
    /// Indicates that a call to <see cref="UIKit.UIApplication.RegisterForRemoteNotifications"/> succeeded.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="deviceToken">The device token.</param>
    public static void DidRegisterRemoteNotifications(this INotifoMobilePush notifo, NSData deviceToken)
    {
        FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
    }

    /// <summary>
    /// Indicates that a call to <see cref="UIKit.UIApplication.RegisterForRemoteNotifications"/> failed.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="error">The error.</param>
    public static void RemoteNotificationRegistrationFailed(this INotifoMobilePush notifo, NSError error)
    {
        FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
    }

    /// <summary>
    /// Method for processing the user's response to a delivered notification.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <param name="center">The shared user notification center object that received the notification.</param>
    /// <param name="response">The user's response to the notification.</param>
    /// <param name="completionHandler">The action to execute when you have finished processing the user's response.</param>
    public static void DidReceiveNotificationResponse(this INotifoMobilePush notifo, UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        if (notifo is InternalIOSPushAdapter adapter)
        {
            adapter.DidReceiveNotificationResponse(response);
        }

        if (CrossFirebasePushNotification.Current is IUNUserNotificationCenterDelegate notificationDelegate)
        {
            notificationDelegate.DidReceiveNotificationResponse(center, response, completionHandler);
        }
    }
}
