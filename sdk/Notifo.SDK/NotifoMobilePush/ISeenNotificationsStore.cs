// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;

namespace Notifo.SDK.NotifoMobilePush
{
    internal interface ISeenNotificationsStore
    {
        ValueTask AddSeenNotificationIdsAsync(int maxCapacity, params Guid[] ids);

        ValueTask<Guid[]> GetSeenNotificationIdsAsync();
    }
}
