// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Notifo.SDK.Resources;
using Plugin.FirebasePushNotification;
using Serilog;

namespace Notifo.SDK.FirebasePlugin
{
    internal class NotifoPushNotificationHandler : DefaultPushNotificationHandler
    {
        public override void OnReceived(IDictionary<string, object> parameters)
        {
            Log.Debug(Strings.ReceivedNotification, parameters);

            if (parameters.ContainsKey(Constants.ApsContentAvailableKey))
            {
                _ = NotifoIO.DidReceivePullRefreshNotificationAsync();
            }

            base.OnReceived(parameters);
        }
    }
}
