// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Notifo.SDK.UnitTests
{
    public class EventsProviderMock : IPushEventsProvider
    {
        public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
        public event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
        public event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;

        protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args) =>
            OnTokenRefresh?.Invoke(this, args);

        public void RaiseOnTokenRefreshEvent()
        {
            var args = new TokenRefreshEventArgs("test token");
            OnRefreshTokenEvent(args);
        }

        protected virtual void OnNotificationReceivedEvent(NotificationDataEventArgs args) =>
            OnNotificationReceived?.Invoke(this, args);

        public void RaiseOnNotificationReceivedEvent()
        {
            var args = new NotificationDataEventArgs(new Dictionary<string, object>());
            OnNotificationReceivedEvent(args);
        }

        protected virtual void OnNotificationOpenedEvent(NotificationResponseEventArgs args) =>
            OnNotificationOpened?.Invoke(this, args);

        public void RaiseOnNotificationOpenedEvent()
        {
            var args = new NotificationResponseEventArgs(new Dictionary<string, object>());
            OnNotificationOpenedEvent(args);
        }
    }
}
