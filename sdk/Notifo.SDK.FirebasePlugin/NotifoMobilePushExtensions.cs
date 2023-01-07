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

namespace Notifo.SDK.FirebasePlugin;

/// <summary>
/// The <see cref="INotifoMobilePush"/> extension methods.
/// </summary>
public static partial class NotifoMobilePushExtensions
{
    private static bool isHandlerRegistered;

    /// <summary>
    /// Use the firebase plugin as the push events provider.
    /// </summary>
    /// <param name="notifo">The <see cref="INotifoMobilePush"/> instance.</param>
    /// <returns>The current instance.</returns>
    public static INotifoMobilePush UseFirebasePluginEventsProvider(this INotifoMobilePush notifo)
    {
        if (isHandlerRegistered)
        {
            return notifo;
        }

        notifo.SetPushEventsProvider(new PluginEventsProvider());
        isHandlerRegistered = true;

        return notifo;
    }
}
