﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK.PushEventProvider
{
    public interface IPushEventsProvider
    {
        event EventHandler<TokenRefreshEventArgs> OnTokenRefresh;

        event EventHandler<NotificationEventArgs> OnNotificationReceived;

        event EventHandler<NotificationEventArgs> OnNotificationOpened;
    }
}
