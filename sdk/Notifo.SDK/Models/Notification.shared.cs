using System;
using System.Collections.Generic;

namespace NotifoIO.SDK
{
	public partial class Notification
	{
		public string Title { get; private set; }
		public string Body { get; private set; }

		internal Notification() { }

		public static Notification FromFirebase(IDictionary<string, object> data)
		{
			var notification = new Notification();

			if (data.ContainsKey(FirebaseTitleKey))
			{
				notification.Title = Convert.ToString(data[FirebaseTitleKey]);
			}

			if (data.ContainsKey(FirebaseBodyKey))
			{
				notification.Body = Convert.ToString(data[FirebaseBodyKey]);
			}

			return notification;
		}
	}
}
