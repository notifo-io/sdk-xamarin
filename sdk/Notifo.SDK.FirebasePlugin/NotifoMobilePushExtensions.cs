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
    /// <summary>
    /// The <see cref="INotifoMobilePush"/> extension methods.
    /// </summary>
    public static class NotifoMobilePushExtensions
    {
        private static readonly Lazy<IPushEventsProvider> CurrentPluginEventsProvider =
            new Lazy<IPushEventsProvider>(() => new PluginEventsProvider(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Use the firebase plugin as the push events provider.
        /// </summary>
        /// <param name="notifoMobilePush">
        /// The <see cref="INotifoMobilePush"/> instance.
        /// </param>
        /// <returns>The current instance.</returns>
        public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifoMobilePush) =>
            notifoMobilePush.SetPushEventsProvider(CurrentPluginEventsProvider.Value);
    }
}
