// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.CommandQueue
{
    internal sealed class TriggerOnStart : ICommandTrigger
    {
        public void Start(ICommandQueue queue)
        {
            queue.Trigger();
        }
    }
}
