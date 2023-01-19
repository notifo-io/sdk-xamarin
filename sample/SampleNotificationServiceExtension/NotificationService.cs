// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Foundation;
using Notifo.SDK;
using Sample.iOS.Shared;
using UserNotifications;

namespace SampleNotificationServiceExtension
{
	[Register("NotificationService")]
	public class NotificationService : UNNotificationServiceExtension
	{
		public Action<UNNotificationContent> ContentHandler { get; set; }

		public UNMutableNotificationContent BestAttemptContent { get; set; }

		protected NotificationService(IntPtr handle)
			: base(handle)
		{
			// Note: this constructor should not contain any initialization logic.
		}

		public override async void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
		{
			ContentHandler = contentHandler;

			//Save the notification and create a mutable copy
			BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

			NotifoIO.Current
				.SetSharedName("group.io.notifo.xamarin.sample")
				.SetRefreshOptions(new PullRefreshOptions())
                .SetNotificationHandler(new NotificationHandler());

            await NotifoIO.Current.DidReceiveNotificationRequestAsync(request, BestAttemptContent);

			// Display the notification.
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
