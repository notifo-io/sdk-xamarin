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
        public HashSet<Guid> Ids { get; set; }

        public string Token { get; set; }

        public async ValueTask ExecuteAsync(
            CancellationToken ct)
        {
            try
            {
                var seenIds = Ids.Select(x => x.ToString()).ToList();

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
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(Strings.TrackingException, ex, this);
                return;
            }
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
                foreach (var id in trackSeen.Ids)
                {
                    Ids.Add(id);
                }

                return true;
            }

            return false;
        }
    }
}
