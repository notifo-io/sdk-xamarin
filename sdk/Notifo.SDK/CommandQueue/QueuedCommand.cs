// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK.CommandQueue;

internal sealed class QueuedCommand
{
    public ICommand Command { get; set; }

    public int Retries { get; set; }

    public Guid CommandId { get; set; }
}
