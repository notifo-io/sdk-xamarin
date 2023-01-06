// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.CommandQueue;

internal sealed class TriggerOnStart : ICommandTrigger
{
    private readonly TimeSpan delay;

    public TriggerOnStart(TimeSpan delay)
    {
        this.delay = delay;
    }

    public void Start(ICommandQueue queue)
    {
        if (delay == TimeSpan.Zero)
        {
            queue.Trigger();
        }
        else
        {
            Task.Delay(delay).ContinueWith(_ => queue.Trigger());
        }
    }
}
