﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK;
using Notifo.SDK.FirebasePlugin;
using Notifo.SDK.NotifoMobilePush;

namespace Sample
{
    public class NotifoStartup : INotifoStartup
    {
        public void ConfigureService(INotifoMobilePush notifo)
        {
            notifo
                .SetBaseUrl(Constants.ApiUrl)
                .SetApiKey(Constants.UserApiKey)
                .UseFirebasePluginEventsProvider()
                .Register();
        }
    }
}
