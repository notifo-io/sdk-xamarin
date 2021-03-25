// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK
{
    public interface IPushEventsProvider
    {
        event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

        event EventHandler<NotificationDataEventArgs> OnNotificationReceived;

        event EventHandler<NotificationResponseEventArgs> OnNotificationOpened;
    }
}
