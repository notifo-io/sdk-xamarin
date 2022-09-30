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
using Serilog;

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
                var trackUserNotificationDto = new TrackNotificationDto
                {
                    Seen = Ids.Select(x => x.ToString()).ToList(),
                    DeviceIdentifier = Token
                };

                await NotifoIO.Current.Notifications.ConfirmMeAsync(trackUserNotificationDto, ct);
            }
            catch (Exception ex)
            {
                ((NotifoMobilePushImplementation)NotifoIO.Current).RaiseError(ex.Message, ex, this);

                Log.Error(Strings.TrackingException, ex);
                return;
            }
        }

        public bool Merge(ICommand other)
        {
            if (other is TrackSeenCommand trackSeen)
            {
                foreach (var id in trackSeen.Ids)
                {
                    Ids.Add(id);
                }

                Token = trackSeen.Token;
                return true;
            }

            return false;
        }
    }
}
