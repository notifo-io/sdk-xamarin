// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.Helpers;
using Notifo.SDK.Resources;
using Serilog;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed partial class NotifoMobilePushImplementation
    {
        private const int Capacity = 500;
        private readonly SemaphoreSlim semaphoreSlim = new (1);
        private SlidingSet<Guid>? seenNotifications;

        public async Task<HashSet<Guid>> GetSeenNotificationsAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var set = await LoadSeenNotificationsAsync();

                return set.ToHashSet();
            }
            catch (Exception ex)
            {
                Log.Error(Strings.TrackingException, ex);
                return new HashSet<Guid>();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public Task TrackNotificationsAsync(params UserNotificationDto[] notifications)
        {
            return TrackNotificationsAsync(notifications.Select(x => x.Id).ToArray());
        }

        public async Task TrackNotificationsAsync(params Guid[] ids)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var set = await LoadSeenNotificationsAsync();

                await seenNotificationsStore.AddSeenNotificationIdsAsync(Capacity, ids);

                foreach (var id in ids)
                {
                    set.Add(id, Capacity);
                }

                commandQueue.Run(new TrackSeenCommand { Ids = ids.ToHashSet(), Token = token });
            }
            catch (Exception ex)
            {
                Log.Error(Strings.TrackingException, ex);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task<SlidingSet<Guid>> LoadSeenNotificationsAsync()
        {
            if (seenNotifications == null)
            {
                seenNotifications = await seenNotificationsStore.GetSeenNotificationIdsAsync();
            }

            return seenNotifications;
        }
    }
}
