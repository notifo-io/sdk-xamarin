namespace NotifoIO.SDK
{
	public class NotificationDataEventArgs
	{
		public Notification Notification { get; }

		public NotificationDataEventArgs(Notification notification)
		{
			Notification = notification;
		}
	}
}
