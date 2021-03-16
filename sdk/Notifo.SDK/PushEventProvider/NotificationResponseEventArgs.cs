using System.Collections.Generic;

namespace NotifoIO.SDK
{
	public class NotificationResponseEventArgs
	{
		public string Identifier { get; }
		public IDictionary<string, object> Data { get; }

		public NotificationResponseEventArgs(IDictionary<string, object> data, string identifier = "")
		{
			Identifier = identifier;
			Data = data;
		}
	}
}
