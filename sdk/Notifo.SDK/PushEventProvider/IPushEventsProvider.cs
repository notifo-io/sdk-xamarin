using System;

namespace NotifoIO.SDK
{
	public interface IPushEventsProvider
	{
		event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

		event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
	}
}
