// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Notifo.SDK.NotifoMobilePush;
using UserNotifications;

namespace Notifo.SDK
{
    public static partial class NotifoIO
    {
        /// <summary>
        /// Method for processing notification before delivery.
        /// </summary>
        /// <param name="request">The request that was received.</param>
        /// <param name="bestAttemptContent">The notification content.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task DidReceiveNotificationRequestAsync(UNNotificationRequest request, UNMutableNotificationContent bestAttemptContent)
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                await notifoMobilePush.DidReceiveNotificationRequestAsync(request, bestAttemptContent);
            }
        }

        /// <summary>
        /// Method for pulling pending notifications.
        /// </summary>
        /// <param name="options">The options for handling the pending notifications pull refresh request.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task DidReceivePullRefreshRequestAsync(PullRefreshOptions? options = null)
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                await notifoMobilePush.DidReceivePullRefreshRequestAsync(options);
            }
        }

        /// <summary>
        /// Method for processing the user's response to a delivered notification.
        /// </summary>
        /// <param name="center">The shared user notification center object that received the notification.</param>
        /// <param name="response">The user's response to the notification.</param>
        /// <param name="completionHandler">The action to execute when you have finished processing the user's response.</param>
        public static void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (Current is NotifoMobilePushImplementation notifoMobilePush)
            {
                notifoMobilePush.DidReceiveNotificationResponse(center, response, completionHandler);
            }
        }
    }
}
