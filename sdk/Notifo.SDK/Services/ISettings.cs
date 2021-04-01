// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Notifo.SDK.Services
{
    internal interface ISettings
    {
        string Token { get; set; }

        bool IsTokenRefreshed { get; set; }

        void TrackNotification(Guid id);
        void TrackNotifications(IEnumerable<Guid> ids);
        bool IsNotificationSeen(Guid id);
    }
}
