using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.FirebasePushNotification;

namespace Sample.Droid
{
	[Application]
	public class MainApplication : Application
	{
		public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();

			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";
				FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
			}

#if DEBUG
			FirebasePushNotificationManager.Initialize(this, resetToken: true);
#else
            FirebasePushNotificationManager.Initialize(this,false);
#endif
		}
	}
}