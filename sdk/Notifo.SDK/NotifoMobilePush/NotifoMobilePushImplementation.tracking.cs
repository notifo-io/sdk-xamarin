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
using Notifo.SDK.Resources;
using Serilog;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed partial class NotifoMobilePushImplementation
    {
        private const int Capacity = 500;
        private readonly SemaphoreSlim semaphoreSlim = new (1);
        private HashSet<Guid>? seenNotificationsSet;
        private LinkedList<Guid>? seenNotificationsList;

        public async Task<HashSet<Guid>> GetSeenNotificationsAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await LoadSeenNotificationsAsync();

                return seenNotificationsSet.ToHashSet();
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
                await LoadSeenNotificationsAsync();

                await seenNotificationsStore.AddSeenNotificationIdsAsync(Capacity, ids);

                while (seenNotificationsSet!.Count > Capacity - ids.Length)
                {
                    var first = seenNotificationsList!.First.Value;

                    seenNotificationsList.RemoveFirst();
                    seenNotificationsSet.Remove(first);
                }

                foreach (var id in ids)
                {
                    Add(id);
                }

                await commandQueue.Run(new TrackSeenCommand { Ids = ids.ToHashSet(), Token = token });
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

        private async Task LoadSeenNotificationsAsync()
        {
            if (seenNotificationsSet == null)
            {
                var loaded = await seenNotificationsStore.GetSeenNotificationIdsAsync();

                seenNotificationsSet = new HashSet<Guid>();
                seenNotificationsList = new LinkedList<Guid>();

                foreach (var id in loaded)
                {
                    Add(id);
                }
            }
        }

        private void Add(Guid id)
        {
            if (seenNotificationsSet!.Add(id))
            {
                seenNotificationsList!.AddLast(id);
            }
        }
    }
}
