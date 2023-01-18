// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using UserNotifications;

namespace Notifo.SDK
{
    public partial interface INotifoMobilePush
    {
        /// <summary>
        /// Sets the notification handler.
        /// </summary>
        /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetNotificationHandler(INotificationHandler notificationHandler);

        /// <summary>
        /// Sets the options that are used when new notifications are pulled.
        /// </summary>
        /// <param name="refreshOptions">The options.</param>
        /// <returns>The current instance.</returns>
        INotifoMobilePush SetRefreshOptions(PullRefreshOptions refreshOptions);

        /// <summary>
        /// Method for processing notification before delivery.
        /// </summary>
        /// <param name="request">The request that was received.</param>
        /// <param name="content">The notification content to enrich.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent content);
    }

    /// <summary>
    /// Provides methods that should not be triggered from user code.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public interface InternalIOSPushAdapter
#pragma warning restore IDE1006 // Naming Styles
    {
        /// <summary>
        /// Method for pulling pending notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DidReceivePullRefreshRequestAsync();

        /// <summary>
        /// Method for processing the user's response to a delivered notification.
        /// </summary>
        /// <param name="response">The user's response to the notification.</param>
        void DidReceiveNotificationResponse(UNNotificationResponse response);
    }
}