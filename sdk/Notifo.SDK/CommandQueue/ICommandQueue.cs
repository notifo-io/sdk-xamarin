// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notifo.SDK.CommandQueue
{
    internal interface ICommandQueue
    {
        event EventHandler<NotificationLogEventArgs> OnLog;

        void Run(ICommand command);

        void Trigger();

        Task CompleteAsync(
            CancellationToken ct);
    }
}
