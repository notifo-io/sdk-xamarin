﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK
{
    /// <summary>
    /// Notification handler interface.
    /// </summary>
    public partial interface INotifoMobilePush
    {
        /// <summary>
        /// Sets the notification handler.
        /// </summary>
        /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
        /// <returns>The current instance.</returns>
        public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler);

        /// <summary>
        /// Sets the capacity of the image cache.
        /// </summary>
        /// <param name="capacity">The capacity in bytes.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetImageCacheCapacity(int capacity);
    }
}
