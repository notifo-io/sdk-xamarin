using System;

namespace NotifoIO.SDK
{
	public interface INotifoMobilePush
	{
		public event EventHandler<NotificationDataEventArgs> OnNotificationReceived;

		INotifoMobilePush SetApiKey(string appId);
		INotifoMobilePush SetBaseUrl(string baseUrl);
		INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
	}
}
