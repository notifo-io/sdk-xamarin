// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Notifo.SDK.Helpers;
using Xamarin.Essentials;

namespace Notifo.SDK.Services
{
    internal class Settings : ISettings
    {
        private static readonly string PrimaryPackageName = Regex.Replace(AppInfo.PackageName, @"\.([^.]*)ServiceExtension$", string.Empty);
        private static readonly string SharedName = $"group.{PrimaryPackageName}.notifo";
        private static readonly string SeenNotificationsKey = "SeenNotifications";

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

        public SlidingSet<Guid> GetSeenNotifications()
        {
            var seenNotifications = new SlidingSet<Guid>(capacity: 500);

            var serialized = Preferences.Get(SeenNotificationsKey, string.Empty, SharedName);
            if (!string.IsNullOrWhiteSpace(serialized))
            {
                seenNotifications = JsonConvert.DeserializeObject<SlidingSet<Guid>>(serialized) ?? seenNotifications;
            }

            return seenNotifications;
        }

        private void SetSeenNotifications(SlidingSet<Guid> seenNotifications)
        {
            var serialized = JsonConvert.SerializeObject(seenNotifications);
            Preferences.Set(SeenNotificationsKey, serialized, SharedName);
        }

        public Task TrackNotificationAsync(Guid id) => TrackNotificationsAsync(new Guid[] { id });

        public async Task TrackNotificationsAsync(IEnumerable<Guid> ids)
        {
            await Semaphore.WaitAsync();
            try
            {
                var seenNotifications = GetSeenNotifications();

                foreach (var id in ids)
                {
                    seenNotifications.Add(id);
                }

                await Task.Run(() => SetSeenNotifications(seenNotifications));
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
