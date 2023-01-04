// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notifo.SDK.CommandQueue
{
    internal interface ICommandStore
    {
        ValueTask<List<QueuedCommand>> GetCommandsAsync();

        ValueTask StoreAsync(QueuedCommand command);

        ValueTask RemoveAsync(Guid id);

        void Clear();
    }
}
