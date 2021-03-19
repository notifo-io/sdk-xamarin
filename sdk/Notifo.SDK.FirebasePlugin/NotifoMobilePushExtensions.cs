// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;

namespace NotifoIO.SDK.FirebasePlugin
{
    public static class NotifoMobilePushExtensions
    {
        private static readonly Lazy<IPushEventsProvider> PluginEventsProvider =
            new Lazy<IPushEventsProvider>(() => new PluginEventsProvider(), LazyThreadSafetyMode.PublicationOnly);

        public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifoMobilePush) =>
            notifoMobilePush.SetPushEventsProvider(PluginEventsProvider.Value);
    }
}
