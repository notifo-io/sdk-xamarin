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
    public class NotifoFirebasePlugin
    {
        public static void Initialize(NSDictionary options, INotifoStartup notifoStartup, NotificationUserCategory[] notificationUserCategories, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, notificationUserCategories, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static void Initialize(NSDictionary options, INotifoStartup notifoStartup, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, pushNotificationHandler, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static void Initialize(NSDictionary options, INotifoStartup notifoStartup, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, autoRegistration);
            notifoStartup.ConfigureService(NotifoIO.Current);
        }

        public static async Task DidReceiveMessageAsync(NSDictionary data)
        {
            Log.Debug(Strings.ReceivedNotification, data);

            if (ContainsPullRefreshRequest(data))
            {
                await NotifoIO.DidReceivePullRefreshRequestAsync();
            }

            FirebasePushNotificationManager.DidReceiveMessage(data);
        }

        public static void DidRegisterRemoteNotifications(NSData deviceToken) =>
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);

        public static void RemoteNotificationRegistrationFailed(NSError error) =>
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

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
