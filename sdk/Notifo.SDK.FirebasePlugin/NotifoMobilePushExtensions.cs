// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using Notifo.SDK.NotifoMobilePush;
using Notifo.SDK.PushEventProvider;

namespace Notifo.SDK.FirebasePlugin
{
    public static class NotifoMobilePushExtensions
    {
        private static readonly Lazy<IPushEventsProvider> CurrentPluginEventsProvider =
            new Lazy<IPushEventsProvider>(() => new PluginEventsProvider(), LazyThreadSafetyMode.PublicationOnly);

        public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifoMobilePush) =>
            notifoMobilePush.SetPushEventsProvider(CurrentPluginEventsProvider.Value);
    }
}
