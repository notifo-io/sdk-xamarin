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
using Notifo.SDK.CommandQueue;
using Notifo.SDK.Resources;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed class TrackSeenCommand : ICommand
    {
        [Obsolete]
        public HashSet<Guid> Ids
        {
            set => IdsAndUrls = value.ToDictionary(x => x, x => (string?)null);
        }

        public Dictionary<Guid, string?> IdsAndUrls { get; set; }

        public string? Token { get; set; }

        public async ValueTask ExecuteAsync(
            CancellationToken ct)
        {
            try
            {
                if (!NotifoIO.Current.IsConfigured)
                {
                    await TrackWithUrlsAsync(ct);
                }
                else
                {
                    await TrackWithIdsAsync(ct);
                }
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
                return;
            }
        }

        private async Task TrackWithUrlsAsync(
            CancellationToken ct)
        {
            var urls = IdsAndUrls.Values.Where(x => x != null).ToList();

            if (urls.Count < IdsAndUrls.Count)
            {
                NotifoIO.Current.RaiseError(Strings.TrackingURLMissing, null, this);
            }

            if (!urls.Any())
            {
                return;
            }

            var httpClient = NotifoIO.Current.CreateHttpClient();

            foreach (var url in urls)
            {
                var response = await httpClient.GetAsync(url, ct);

                response.EnsureSuccessStatusCode();
            }
        }

        private async Task TrackWithIdsAsync(
            CancellationToken ct)
        {
            var seenIds = IdsAndUrls.Select(x => x.Key.ToString()).ToList();

            var trackRequest = new TrackNotificationDto
            {
                // Track all notifications at once.
                Seen = seenIds,

                // Track individual channels.
                Channel = Providers.MobilePush
            };

            // Track twice to support backwards compatibility with older Notifo versions.
            await TrackByTokenAsync(trackRequest, ct);
            await TrackByIdentifierAsync(trackRequest, ct);
        }

        private async Task TrackByTokenAsync(TrackNotificationDto trackRequest,
            CancellationToken ct)
        {
            trackRequest.DeviceIdentifier = Token;

            await NotifoIO.Current.Notifications.ConfirmMeAsync(trackRequest, ct);
        }

        private async Task TrackByIdentifierAsync(TrackNotificationDto trackRequest,
            CancellationToken ct)
        {
            trackRequest.DeviceIdentifier = Device.DeviceIdentifier;

            await NotifoIO.Current.Notifications.ConfirmMeAsync(trackRequest, ct);
        }

        public bool Merge(ICommand other)
        {
            // Do not merge commands with different mobile tokens.
            if (other is TrackSeenCommand trackSeen && string.Equals(Token, trackSeen.Token))
            {
                foreach (var (id, url) in trackSeen.IdsAndUrls)
                {
                    // Ensure that we do not override the URL with null.
                    IdsAndUrls[id] = url ?? IdsAndUrls.GetValueOrDefault(id);
                }

                return true;
            }

            return false;
        }
    }
}
