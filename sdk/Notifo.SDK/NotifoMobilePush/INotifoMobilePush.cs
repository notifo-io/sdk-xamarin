// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK
{
    public interface INotifoMobilePush
    {
        event EventHandler<NotificationDataEventArgs> OnNotificationReceived;
        event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;
        INotifoMobilePush SetApiKey(string appId);
        INotifoMobilePush SetBaseUrl(string baseUrl);
        INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
        void Register();
    }
}
