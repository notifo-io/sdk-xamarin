using Foundation;
using Plugin.FirebasePushNotification;

namespace NotifoIO.SDK.FirebasePlugin
{
	public partial class NotifoFirebasePlugin
	{
		public static void Initialize(NSDictionary options, NotificationUserCategory[] notificationUserCategories, bool autoRegistration = true)
		{
			FirebasePushNotificationManager.Initialize(options, notificationUserCategories, autoRegistration);
			Notifo.Current.UseFirebasePluginEventsProvider();
		}

		public static void Initialize(NSDictionary options, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true)
		{
			FirebasePushNotificationManager.Initialize(options, pushNotificationHandler, autoRegistration);
			Notifo.Current.UseFirebasePluginEventsProvider();
		}

		public static void Initialize(NSDictionary options, bool autoRegistration = true)
		{
			FirebasePushNotificationManager.Initialize(options, autoRegistration);
			Notifo.Current.UseFirebasePluginEventsProvider();
		}

		public static void DidReceiveMessage(NSDictionary data) => 		
			FirebasePushNotificationManager.DidReceiveMessage(data);	
		
		public static void DidRegisterRemoteNotifications(NSData deviceToken) =>		
			FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);		

		public static void RemoteNotificationRegistrationFailed(NSError error) => 		
			FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);		
	}
}
