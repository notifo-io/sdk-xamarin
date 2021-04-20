// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.UnitTests.Mocks
{
    public class EventsProviderMock : IPushEventsProvider
    {
        public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;
        public event EventHandler<NotificationEventArgs> OnNotificationOpened;
        public string Token => "test token";

        protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args) =>
            OnTokenRefresh?.Invoke(this, args);

        public void RaiseOnTokenRefreshEvent()
        {
            var args = new TokenRefreshEventArgs("test token");
            OnRefreshTokenEvent(args);
        }

        protected virtual void OnNotificationReceivedEvent(NotificationEventArgs args) =>
            OnNotificationReceived?.Invoke(this, args);

        public void RaiseOnNotificationReceivedEvent()
        {
            var args = new NotificationEventArgs(new Dictionary<string, object>());
            OnNotificationReceivedEvent(args);
        }

        protected virtual void OnNotificationOpenedEvent(NotificationEventArgs args) =>
            OnNotificationOpened?.Invoke(this, args);

        public void RaiseOnNotificationOpenedEvent()
        {
            var args = new NotificationEventArgs(new Dictionary<string, object>());
            OnNotificationOpenedEvent(args);
        }
    }
}
