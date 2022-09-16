// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;

namespace Notifo.SDK.CommandQueue
{
    internal sealed class TrigerPeriodically : ICommandTrigger, IDisposable
    {
        private readonly TimeSpan interval;
        private Timer? timer;

        public TrigerPeriodically(TimeSpan interval)
        {
            this.interval = interval;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public void Start(ICommandQueue queue)
        {
            timer ??= new Timer(x =>
            {
                queue.Trigger();
            });

            timer.Change((int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
        }
    }
}
