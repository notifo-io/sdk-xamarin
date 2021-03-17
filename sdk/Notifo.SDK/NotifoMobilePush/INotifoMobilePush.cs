using System;

namespace NotifoIO.SDK
{
	public interface INotifoMobilePush
	{
		event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
		event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;
		INotifoMobilePush SetApiKey(string appId);
		INotifoMobilePush SetBaseUrl(string baseUrl);
		INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
	}
}
