using System;
using Foundation;
using Plugin.FirebasePushNotification;
using UIKit;

namespace Sample.iOS
{
	[Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

			FirebasePushNotificationManager.Initialize(options, true);

			return base.FinishedLaunching(app, options);
        }

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			FirebasePushNotificationManager.DidReceiveMessage(userInfo);
			completionHandler(UIBackgroundFetchResult.NewData);
		}
	}
}
