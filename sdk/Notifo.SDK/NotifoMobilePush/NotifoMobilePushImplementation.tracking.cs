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

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed partial class NotifoMobilePushImplementation
    {
        private const int Capacity = 500;
        private readonly SemaphoreSlim semaphoreSlim = new (1);

        public async Task<HashSet<Guid>> GetSeenNotificationsAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                // Always load the notifications from the preferences, because they could have been modified by the service extension.
                var loaded = await seenNotificationsStore.GetSeenNotificationIdsAsync();

                return loaded.ToHashSet();
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
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
                // Always load the notifications from the preferences, because they could have been modified by the service extension.
                var loaded = await seenNotificationsStore.GetSeenNotificationIdsAsync();

                // Store the seen notifications immediately as a cache, if the actual command to the server fails.
                await seenNotificationsStore.AddSeenNotificationIdsAsync(Capacity, ids);

                foreach (var id in ids)
                {
                    loaded.Add(id, Capacity);
                }

                // Track all notifications with one HTTP request.
                commandQueue.Run(new TrackSeenCommand { Ids = ids.ToHashSet(), Token = token });
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
                return;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
