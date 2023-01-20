// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using Plugin.Connectivity.Abstractions;

namespace Notifo.SDK.CommandQueue
{
    internal sealed class TriggerPeriodically : ICommandTrigger, IDisposable
    {
        private readonly TimeSpan interval;
        private readonly IConnectivity connectivity;
        private Timer? timer;

        public TriggerPeriodically(TimeSpan interval, IConnectivity connectivity)
        {
            this.interval = interval;
            this.connectivity = connectivity;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public void Start(ICommandQueue queue)
        {
            timer ??= new Timer(x =>
            {
                if (connectivity.IsConnected)
                {
                    queue.Trigger();
                }
            });

            timer.Change((int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
        }
    }
}