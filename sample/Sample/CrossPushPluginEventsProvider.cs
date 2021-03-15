using System;
using NotifoIO.SDK;
using Plugin.FirebasePushNotification;

namespace Sample
{
	public class CrossPushPluginEventsProvider : IPushEventsProvider
	{
		public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
		public event EventHandler<NotificationDataEventArgs> OnNotificationReceived;

		public CrossPushPluginEventsProvider(IFirebasePushNotification firebasePushNotification)
		{
			firebasePushNotification.OnTokenRefresh += FirebasePushNotification_OnTokenRefresh;
			firebasePushNotification.OnNotificationReceived += FirebasePushNotification_OnNotificationReceived;
		}

		private void FirebasePushNotification_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
		{
			var args = new TokenRefreshEventArgs(e.Token);
			OnRefreshTokenEvent(args);
		}

		protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args)
		{
			var handler = OnTokenRefresh;
			handler?.Invoke(this, args);
		}

		private void FirebasePushNotification_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
		{
			var notification = Notification.FromFirebase(e.Data);
			var args = new NotificationDataEventArgs(notification);
			OnNotificationReceivedEvent(args);
		}

		protected virtual void OnNotificationReceivedEvent(NotificationDataEventArgs args)
		{
			var handler = OnNotificationReceived;
			handler?.Invoke(this, args);
		}
	}
}
