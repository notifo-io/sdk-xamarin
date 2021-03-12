using System;
using NotifoIO.SDK;
using Plugin.FirebasePushNotification;

namespace Sample
{
	public class CrossPushPluginEventsProvider : IPushEventsProvider
	{
		public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

		public CrossPushPluginEventsProvider(IFirebasePushNotification firebasePushNotification)
		{
			firebasePushNotification.OnTokenRefresh += FirebasePushNotification_OnTokenRefresh;
		}

		private void FirebasePushNotification_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
		{
			var args = new TokenRefreshEventArgs
			{
				Token = e.Token
			};
			OnRefreshTokenEvent(args);
		}

		protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args)
		{
			var handler = OnTokenRefresh;
			handler?.Invoke(this, args);
		}
	}
}
