// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.NotifoMobilePush
{
    public interface INotifoMobilePush
    {
        event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
        event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;
        INotifoMobilePush SetApiKey(string appId);
        INotifoMobilePush SetBaseUrl(string baseUrl);
        INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
        void Register();
        INotificationsClient Notifications { get; }
    }
}
