// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.SDK.NotifoMobilePush;

namespace Notifo.SDK
{
    /// <summary>
    /// Provides a place to configure Notifo mobile push service.
    /// </summary>
    public interface INotifoStartup
    {
        /// <summary>
        /// Configure the Notifo mobile push service.
        /// </summary>
        /// <param name="notifo">The Notifo mobile push service.</param>
        void ConfigureService(INotifoMobilePush notifo);
    }
}
