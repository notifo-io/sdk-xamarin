// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.CommandQueue;

internal interface ICommandStore
{
    ValueTask<List<QueuedCommand>> GetCommandsAsync();

    ValueTask StoreAsync(QueuedCommand command);

    ValueTask RemoveAsync(Guid id);

    void Clear();
}
