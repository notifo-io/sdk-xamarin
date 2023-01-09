// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifo.SDK.Extensions;
using Notifo.SDK.PushEventProvider;
using Plugin.FirebasePushNotification;

namespace Notifo.SDK.FirebasePlugin;

internal class PluginEventsProvider : IPushEventsProvider
{
    public event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

    public event EventHandler<Notifo.SDK.NotificationEventArgs> OnNotificationReceived;

    public event EventHandler<Notifo.SDK.NotificationEventArgs> OnNotificationOpened;

    public event EventHandler<Notifo.SDK.NotificationLogEventArgs> OnLog;

    public PluginEventsProvider()
    {
        CrossFirebasePushNotification.Current.OnTokenRefresh += FirebasePushNotification_OnTokenRefresh;
        CrossFirebasePushNotification.Current.OnNotificationReceived += FirebasePushNotification_OnNotificationReceived;
        CrossFirebasePushNotification.Current.OnNotificationOpened += FirebasePushNotification_OnNotificationOpened;
        CrossFirebasePushNotification.Current.OnNotificationError += Current_OnNotificationError;
    }

    private void FirebasePushNotification_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
    {
        OnTokenRefresh?.Invoke(this, new TokenRefreshEventArgs(e.Token));
    }

    private void Current_OnNotificationError(object source, FirebasePushNotificationErrorEventArgs e)
    {
        OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Error, source, e.Message, null, null));
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

    public Task<string> GetTokenAsync()
    {
        return CrossFirebasePushNotification.Current.GetTokenAsync();    
    }
}
