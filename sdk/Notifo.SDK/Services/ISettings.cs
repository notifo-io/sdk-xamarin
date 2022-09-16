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
        ValueTask SetTokenAsync(string token);

        ValueTask<string> GetTokenAsync();

        ValueTask SetTokenRefreshedAsync(bool isTokenRefreshed);

        ValueTask<bool> GetTokenRefreshedAsync();

        ValueTask SetSeenNotificationIdsAsync(params Guid[] ids);

        ValueTask<ISet<Guid>> GetSeenNotificationIdsAsync();

        void Clear();
    }
}
