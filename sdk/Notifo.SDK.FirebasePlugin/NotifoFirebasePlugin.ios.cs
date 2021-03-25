// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Foundation;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    public class NotifoFirebasePlugin
    {
        public static void Initialize(NSDictionary options, NotificationUserCategory[] notificationUserCategories, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, notificationUserCategories, autoRegistration);
            NotifoIO.Current.UseFirebasePluginEventsProvider();
        }

        public static void Initialize(NSDictionary options, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, pushNotificationHandler, autoRegistration);
            NotifoIO.Current.UseFirebasePluginEventsProvider();
        }

        public static void Initialize(NSDictionary options, bool autoRegistration = true)
        {
            FirebasePushNotificationManager.Initialize(options, autoRegistration);
            NotifoIO.Current.UseFirebasePluginEventsProvider();
        }

        public static void DidReceiveMessage(NSDictionary data) =>
            FirebasePushNotificationManager.DidReceiveMessage(data);

        public static void DidRegisterRemoteNotifications(NSData deviceToken) =>
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);

        public static void RemoteNotificationRegistrationFailed(NSError error) =>
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
    }
}
