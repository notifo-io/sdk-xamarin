using System;
using System.Collections.Generic;

namespace NotifoIO.SDK.UnitTests
{
	public class EventsProviderMock : IPushEventsProvider
	{
		public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
		public event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
		public event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;

		protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args)
		{
			var handler = OnTokenRefresh;
			handler?.Invoke(this, args);
		}

		public void RaiseOnTokenRefreshEvent()
		{
			var args = new TokenRefreshEventArgs("test token");
			OnRefreshTokenEvent(args);
		}

		protected virtual void OnNotificationReceivedEvent(NotificationDataEventArgs args)
		{
			var handler = OnNotificationReceived;
			handler?.Invoke(this, args);
		}

		public void RaiseOnNotificationReceivedEvent()
		{
			var args = new NotificationDataEventArgs(new Dictionary<string, object>());
			OnNotificationReceivedEvent(args);
		}

		protected virtual void OnNotificationOpenedEvent(NotificationResponseEventArgs args)
		{
			var handler = OnNotificationOpened;
			handler?.Invoke(this, args);
		}

		public void RaiseOnNotificationOpenedEvent()
		{
			var args = new NotificationResponseEventArgs(new Dictionary<string, object>());
			OnNotificationOpenedEvent(args);
		}
	}
}
