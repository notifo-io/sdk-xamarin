// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK
{
    public partial interface INotifoMobilePush
    {
        /// <summary>
        /// Sets the notification handler.
        /// </summary>
        /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
        /// <returns>The current instance.</returns>
        public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler);
    }
}
