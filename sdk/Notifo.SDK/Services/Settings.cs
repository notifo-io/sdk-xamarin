// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xamarin.Essentials;

namespace Notifo.SDK
{
    internal class Settings : ISettings
    {
        private const int SeenNotificationsMaxCapacity = 10_000;
        private static readonly string SharedName = $"{AppInfo.PackageName}.notifo";
        private static readonly object Locker = new object();

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

        private Dictionary<Guid, DateTime> seenNotifications;
        private Dictionary<Guid, DateTime> SeenNotifications
        {
            get
            {
                if (seenNotifications == null)
                {
                    seenNotifications = new Dictionary<Guid, DateTime>();

                    var serialized = Preferences.Get(nameof(SeenNotifications), string.Empty, SharedName);
                    if (!string.IsNullOrWhiteSpace(serialized))
                    {
                        seenNotifications = JsonSerializer.Deserialize<Dictionary<Guid, DateTime>>(serialized) ?? seenNotifications;
                    }
                }

                return seenNotifications;
            }
            set
            {
                seenNotifications = value;

                var serialized = JsonSerializer.Serialize(seenNotifications);
                Preferences.Set(nameof(SeenNotifications), serialized, SharedName);
            }
        }

        public bool IsNotificationSeen(Guid id) => SeenNotifications.ContainsKey(id);

        public void TrackNotification(Guid id)
        {
            lock (Locker)
            {
                var notifications = EnsureRespectsMaxCapacity(SeenNotifications, SeenNotificationsMaxCapacity);

                notifications[id] = DateTime.Now;
                SeenNotifications = notifications;
            }
        }

        public void TrackNotifications(IEnumerable<Guid> ids)
        {
            lock (Locker)
            {
                var notifications = EnsureRespectsMaxCapacity(SeenNotifications, SeenNotificationsMaxCapacity);

                foreach (var id in ids) {
                    notifications[id] = DateTime.Now;
                }

                SeenNotifications = notifications;
            }
        }

        private Dictionary<Guid, DateTime> EnsureRespectsMaxCapacity(Dictionary<Guid, DateTime> dictionary, int maxCapacity)
        {
            if (dictionary.Count < maxCapacity)
            {
                return dictionary;
            }

            var keys = dictionary
                .OrderBy(x => x.Value)
                .Take(maxCapacity / 2)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }

            return dictionary;
        }
    }
}
