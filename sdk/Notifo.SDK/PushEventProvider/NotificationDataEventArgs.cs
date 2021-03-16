using System.Collections.Generic;

namespace NotifoIO.SDK
{
	public class NotificationDataEventArgs
	{
		public IDictionary<string, object> Data { get; }

		public NotificationDataEventArgs(IDictionary<string, object> data)
		{
			Data = data;
		}
	}
}
