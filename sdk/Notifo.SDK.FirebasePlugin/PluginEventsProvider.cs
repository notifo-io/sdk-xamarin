// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Notifo.SDK.PushEventProvider;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    internal class PluginEventsProvider : IPushEventsProvider
    {
        public event EventHandler<TokenRefreshEventArgs>? OnTokenRefresh;
        public event EventHandler<NotificationEventArgs>? OnNotificationReceived;
        public event EventHandler<NotificationEventArgs>? OnNotificationOpened;

        public PluginEventsProvider()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += FirebasePushNotification_OnTokenRefresh;
            CrossFirebasePushNotification.Current.OnNotificationReceived += FirebasePushNotification_OnNotificationReceived;
            CrossFirebasePushNotification.Current.OnNotificationOpened += FirebasePushNotification_OnNotificationOpened;
        }

        private void FirebasePushNotification_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            var args = new TokenRefreshEventArgs(e.Token);
            OnRefreshTokenEvent(args);
        }

        protected virtual void OnRefreshTokenEvent(TokenRefreshEventArgs args) =>
            OnTokenRefresh?.Invoke(this, args);

        private void FirebasePushNotification_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            if (!IsNotificationData(e.Data))
            {
                return;
            }

            var args = new NotificationEventArgs(new Dictionary<string, object>(e.Data));
            OnNotificationReceivedEvent(args);
        }

        protected virtual void OnNotificationReceivedEvent(NotificationEventArgs args) =>
            OnNotificationReceived?.Invoke(this, args);

        private void FirebasePushNotification_OnNotificationOpened(object source, FirebasePushNotificationResponseEventArgs e)
        {
            if (!IsNotificationData(e.Data))
            {
                return;
            }

            var args = new NotificationEventArgs(new Dictionary<string, object>(e.Data));
            OnNotificationOpenedEvent(args);
        }

        protected virtual void OnNotificationOpenedEvent(NotificationEventArgs args) =>
            OnNotificationOpened?.Invoke(this, args);

        private bool IsNotificationData(IDictionary<string, object> data) =>
            data?.ContainsKey(Constants.IdKey) ?? false;
    }
}
