// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK.CommandQueue
{
    internal interface ICommandQueue
    {
        event EventHandler<NotificationErrorEventArgs> OnError;

        void Run(ICommand command);

        void Trigger();
    }
}
