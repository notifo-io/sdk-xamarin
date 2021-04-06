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
    public interface INotifoMobilePush : INotifoClient
    {
        event EventHandler<NotificationEventArgs> OnNotificationReceived;
        event EventHandler<NotificationEventArgs> OnNotificationOpened;
        INotifoMobilePush SetApiKey(string appId);
        INotifoMobilePush SetBaseUrl(string baseUrl);
        INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
        void Register();
    }
}
