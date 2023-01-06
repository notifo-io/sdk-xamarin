// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Notifo.SDK.CommandQueue;

internal interface ICommand
{
    ValueTask ExecuteAsync(
        CancellationToken ct);

    bool Merge(ICommand other);
}
