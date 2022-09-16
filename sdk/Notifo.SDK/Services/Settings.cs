// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Notifo.SDK.Services
{
    internal class Settings : ISettings
    {
        private const int NotificationsCapacity = 500;
        private const string KeyToken = "Token";
        private const string KeySeenNotifications = "SeenNotifications";
        private const string KeyIsTokenRefreshed = "IsTokenRefreshed";
        private static readonly string PrimaryPackageName = Regex.Replace(AppInfo.PackageName, @"\.([^.]*)ServiceExtension$", string.Empty);
        private static readonly string SharedName = $"group.{PrimaryPackageName}.notifo";

        public ValueTask SetTokenAsync(string token)
        {
            Preferences.Set(KeyToken, token, SharedName);

            return default;
        }

        public ValueTask<string> GetTokenAsync()
        {
            var result = Preferences.Get(KeyToken, string.Empty, SharedName);

            return new ValueTask<string>(result);
        }

        public ValueTask SetTokenRefreshedAsync(bool isTokenRefreshed)
        {
            Preferences.Set(KeyIsTokenRefreshed, isTokenRefreshed, SharedName);

            return default;
        }

        public ValueTask<bool> GetTokenRefreshedAsync()
        {
            var result = Preferences.Get(KeyIsTokenRefreshed, false, SharedName);

            return new ValueTask<bool>(result);
        }

        public async ValueTask SetSeenNotificationIdsAsync(params Guid[] ids)
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    var seenNotifications = GetSeenNotificationsCore();

                    foreach (var id in ids)
                    {
                        if (seenNotifications.Count == NotificationsCapacity)
                        {
                            seenNotifications.RemoveFirst();
                        }

                        seenNotifications.AddLast(id);
                    }
                }
            });
        }

        public ValueTask<ISet<Guid>> GetSeenNotificationIdsAsync()
        {
            var result = GetSeenNotificationsCore().ToHashSet();

            return new ValueTask<ISet<Guid>>(result);
        }

        private LinkedList<Guid> GetSeenNotificationsCore()
        {
            var serialized = Preferences.Get(KeySeenNotifications, string.Empty, SharedName);

            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return JsonConvert.DeserializeObject<LinkedList<Guid>>(serialized);
            }

            return new LinkedList<Guid>();
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }
    }
}
