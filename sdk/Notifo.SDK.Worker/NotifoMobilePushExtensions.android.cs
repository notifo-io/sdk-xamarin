﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using AndroidX.Work;

namespace Notifo.SDK
{
    /// <summary>
    /// The <see cref="INotifoMobilePush"/> extension methods.
    /// </summary>
    public static class NotifoMobilePushExtensions
    {
        private static bool isWorkerRegistered;

        /// <summary>
        /// Register a worker to update the mobile push automatically.
        /// </summary>
        /// <param name="notifoMobilePush">The <see cref="INotifoMobilePush"/> instance.</param>
        /// <param name="period">The update period.</param>
        /// <returns>The current instance.</returns>
        public static INotifoMobilePush UseTokenUpdateWorker(this INotifoMobilePush notifoMobilePush, TimeSpan period = default(TimeSpan))
        {
            if (isWorkerRegistered)
            {
                return notifoMobilePush;
            }

            isWorkerRegistered = true;

            if (period == default(TimeSpan))
            {
                period = TimeSpan.FromDays(30);
            }

            var workerRequest = PeriodicWorkRequest.Builder.From<UpdateWorker>(period).Build();

            WorkManager.Instance.Enqueue(workerRequest);

            return notifoMobilePush;
        }
    }
}