// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Notifo.SDK.Helpers;
using Xamarin.Essentials;

namespace Notifo.SDK.Services
{
    internal class Settings : ISettings
    {
        private static readonly string SharedName = $"{AppInfo.PackageName}.notifo";
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public string Token
        {
            get => Preferences.Get(nameof(Token), string.Empty, SharedName);
            set => Preferences.Set(nameof(Token), value, SharedName);
        }

        public bool IsTokenRefreshed
        {
            get => Preferences.Get(nameof(IsTokenRefreshed), false, SharedName);
            set => Preferences.Set(nameof(IsTokenRefreshed), value, SharedName);
        }

        private SlidingSet<Guid> seenNotifications;
        private SlidingSet<Guid> SeenNotifications
        {
            get
            {
                if (seenNotifications == null)
                {
                    seenNotifications = new SlidingSet<Guid>(capacity: 500);

                    var serialized = Preferences.Get(nameof(SeenNotifications), string.Empty, SharedName);
                    if (!string.IsNullOrWhiteSpace(serialized))
                    {
                        seenNotifications = JsonConvert.DeserializeObject<SlidingSet<Guid>>(serialized) ?? seenNotifications;
                    }
                }

                return seenNotifications;
            }
            set
            {
                seenNotifications = value;

                var serialized = JsonConvert.SerializeObject(seenNotifications);
                Preferences.Set(nameof(SeenNotifications), serialized, SharedName);
            }
        }

        public bool IsNotificationSeen(Guid id) => SeenNotifications.Contains(id);

        public async Task TrackNotificationAsync(Guid id)
        {
            await Semaphore.WaitAsync();
            try
            {
                SeenNotifications.Add(id);
                await Task.Run(() => SeenNotifications = SeenNotifications);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task TrackNotificationsAsync(IEnumerable<Guid> ids)
        {
            await Semaphore.WaitAsync();
            try
            {
                foreach (var id in ids)
                {
                    SeenNotifications.Add(id);
                }

                await Task.Run(() => SeenNotifications = SeenNotifications);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
