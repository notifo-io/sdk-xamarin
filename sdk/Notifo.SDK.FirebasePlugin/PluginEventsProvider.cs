// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Notifo.SDK.Extensions;
using Notifo.SDK.PushEventProvider;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin
{
    internal class PluginEventsProvider : IPushEventsProvider
    {
        public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;
        public event EventHandler<Notifo.SDK.NotificationEventArgs> OnNotificationReceived;
        public event EventHandler<Notifo.SDK.NotificationEventArgs> OnNotificationOpened;
        public event EventHandler<Notifo.SDK.NotificationErrorEventArgs> OnError;

        public PluginEventsProvider()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += FirebasePushNotification_OnTokenRefresh;
            CrossFirebasePushNotification.Current.OnNotificationReceived += FirebasePushNotification_OnNotificationReceived;
            CrossFirebasePushNotification.Current.OnNotificationOpened += FirebasePushNotification_OnNotificationOpened;
            CrossFirebasePushNotification.Current.OnNotificationError += Current_OnNotificationError;
        }

        public string Token => CrossFirebasePushNotification.Current.Token;

        private void FirebasePushNotification_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            OnTokenRefresh?.Invoke(this, new TokenRefreshEventArgs(e.Token));
        }

        private void Current_OnNotificationError(object source, FirebasePushNotificationErrorEventArgs e)
        {
            OnError?.Invoke(this, new NotificationErrorEventArgs(e.Message, null, source));
        }

        private void FirebasePushNotification_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            if (!IsNotificationData(e.Data))
            {
                return;
            }

            var args =
                new NotificationEventArgs(
                    new UserNotificationDto().FromDictionary(new Dictionary<string, object>(e.Data)));

            OnNotificationReceived?.Invoke(this, args);
        }

        private void FirebasePushNotification_OnNotificationOpened(object source, FirebasePushNotificationResponseEventArgs e)
        {
            if (!IsNotificationData(e.Data))
            {
                return;
            }

            var args =
                new NotificationEventArgs(
                    new UserNotificationDto().FromDictionary(new Dictionary<string, object>(e.Data)));

            OnNotificationOpened?.Invoke(this, args);
        }

        private bool IsNotificationData(IDictionary<string, object> data)
        {
            if (data == null)
            {
                return false;
            }

            return data.ContainsKey(Constants.SubjectKey) || data.ContainsKey(Constants.ApsAlertTitleKey);
        }
    }
}
