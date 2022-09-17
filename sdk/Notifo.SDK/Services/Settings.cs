﻿// ==========================================================================
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
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Helpers;
using Notifo.SDK.NotifoMobilePush;
using Xamarin.Essentials;

namespace Notifo.SDK.Services
{
    internal sealed class Settings : ISeenNotificationsStore, ICommandStore
    {
        private const string KeyCommand = "CommandV2";
        private const string KeySeenNotifications = "SeenNotificationsV2";
        private static readonly string PrimaryPackageName = Regex.Replace(AppInfo.PackageName, @"\.([^.]*)ServiceExtension$", string.Empty);
        private static readonly string SharedName = $"group.{PrimaryPackageName}.notifo";

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public async ValueTask AddSeenNotificationIdsAsync(int maxCapacity, params Guid[] ids)
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    var seenNotifications = GetSeenNotificationsCore();

                    foreach (var id in ids)
                    {
                        seenNotifications.Add(id, maxCapacity);
                    }

                    StoreSeenNotificationsCore(seenNotifications);
                }
            });
        }

        public async ValueTask<SlidingSet<Guid>> GetSeenNotificationIdsAsync()
        {
            return await Task.Run(() =>
            {
                return GetSeenNotificationsCore();
            });
        }

        public async ValueTask<List<QueuedCommand>> GetCommandsAsync()
        {
            return await Task.Run(() =>
            {
                return GetCommandsCore().Values.ToList();
            });
        }

        public async ValueTask RemoveAsync(Guid id)
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    var commands = GetCommandsCore();

                    commands.Remove(id);

                    StoreCommandsCore(commands);
                }
            });
        }

        public async ValueTask StoreAsync(QueuedCommand command)
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    var commands = GetCommandsCore();

                    commands[command.CommandId] = command;

                    StoreCommandsCore(commands);
                }
            });
        }

        private SlidingSet<Guid> GetSeenNotificationsCore()
        {
            var serialized = Preferences.Get(KeySeenNotifications, string.Empty, SharedName);

            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return JsonConvert.DeserializeObject<SlidingSet<Guid>>(serialized, SerializerSettings)!;
            }

            return new SlidingSet<Guid>();
        }

        private void StoreSeenNotificationsCore(SlidingSet<Guid> value)
        {
            var json = JsonConvert.SerializeObject(value, SerializerSettings);

            Preferences.Set(KeySeenNotifications, json, SharedName);
        }

        private Dictionary<Guid, QueuedCommand> GetCommandsCore()
        {
            var serialized = Preferences.Get(KeyCommand, string.Empty, SharedName);

            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return JsonConvert.DeserializeObject<Dictionary<Guid, QueuedCommand>>(serialized, SerializerSettings)!;
            }

            return new Dictionary<Guid, QueuedCommand>();
        }

        private void StoreCommandsCore(Dictionary<Guid, QueuedCommand> value)
        {
            var json = JsonConvert.SerializeObject(value, SerializerSettings);

            Preferences.Set(KeyCommand, json, SharedName);
        }
    }
}
