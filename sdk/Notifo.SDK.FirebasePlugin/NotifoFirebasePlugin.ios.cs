// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Foundation;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using Serilog;
using UserNotifications;

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
        /// <param name="options">Application launch options.</param>
        /// <param name="notifoStartup">The <see cref="INotifoStartup"/> implementation.</param>
        /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
        /// <param name="autoRegistration">Automatically register for push notifications.</param>
        public static void Initialize(NSDictionary options, INotifoStartup notifoStartup, INotificationHandler? notificationHandler = null, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, autoRegistration);

            NotifoIO.Current.SetNotificationHandler(notificationHandler);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        /// <summary>
        /// Invoked when the application received a remote notification.
        /// </summary>
        /// <param name="data">The notification data dictionary.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task DidReceiveMessageAsync(NSDictionary data)
        {
            Log.Debug(Strings.ReceivedNotification, data);

            if (ContainsPullRefreshRequest(data))
            {
                await NotifoIO.DidReceivePullRefreshRequestAsync();
            }

            FirebasePushNotificationManager.DidReceiveMessage(data);
        }

        /// <summary>
        /// Indicates that a call to <see cref="UIKit.UIApplication.RegisterForRemoteNotifications"/> succeeded.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        public static void DidRegisterRemoteNotifications(NSData deviceToken) =>
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);

        /// <summary>
        /// Indicates that a call to <see cref="UIKit.UIApplication.RegisterForRemoteNotifications"/> failed.
        /// </summary>
        /// <param name="error">The error.</param>
        public static void RemoteNotificationRegistrationFailed(NSError error) =>
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        /// <summary>
        /// Method for processing the user's response to a delivered notification.
        /// </summary>
        /// <param name="center">The shared user notification center object that received the notification.</param>
        /// <param name="response">The user's response to the notification.</param>
        /// <param name="completionHandler">The action to execute when you have finished processing the user's response.</param>
        public static void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            NotifoIO.DidReceiveNotificationResponse(center, response, completionHandler);

            if (CrossFirebasePushNotification.Current is IUNUserNotificationCenterDelegate notificationDelegate)
            {
                notificationDelegate.DidReceiveNotificationResponse(center, response, completionHandler);
            }
        }

        private static bool ContainsPullRefreshRequest(NSDictionary data)
        {
            var aps = data?.ObjectForKey(new NSString(Constants.ApsKey)) as NSDictionary;

            return aps != null && aps.ContainsKey(new NSString(Constants.ContentAvailableKey));
        }
    }
}
