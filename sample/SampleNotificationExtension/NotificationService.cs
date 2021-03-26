﻿using System;
using Foundation;
using Notifo.SDK;
using UserNotifications;

namespace SampleNotificationExtension
{
	[Register("NotificationService")]
	public class NotificationService : UNNotificationServiceExtension
	{
		Action<UNNotificationContent> ContentHandler { get; set; }
		UNMutableNotificationContent BestAttemptContent { get; set; }

		protected NotificationService(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
		{
			ContentHandler = contentHandler;
			BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

			ContentHandler(BestAttemptContent);
		}

		public override void TimeWillExpire()
		{
			// Called just before the extension will be terminated by the system.
			// Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

			ContentHandler(BestAttemptContent);
		}
	}
}
