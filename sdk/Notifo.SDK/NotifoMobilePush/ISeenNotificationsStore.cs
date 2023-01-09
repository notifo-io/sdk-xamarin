// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK.Helpers;

namespace Notifo.SDK.NotifoMobilePush;

internal interface ISeenNotificationsStore
{
    ValueTask AddSeenNotificationIdsAsync(int maxCapacity, IEnumerable<Guid> ids);

    ValueTask<SlidingSet<Guid>> GetSeenNotificationIdsAsync();

    void Clear();
}
