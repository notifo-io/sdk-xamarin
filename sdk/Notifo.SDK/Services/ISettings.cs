// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notifo.SDK.Services
{
    internal interface ISettings
    {
        string Token { get; set; }

        bool IsTokenRefreshed { get; set; }

        Task TrackNotificationAsync(Guid id);

        Task TrackNotificationsAsync(IEnumerable<Guid> ids);

        bool IsNotificationSeen(Guid id);
    }
}
