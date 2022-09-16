// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;

namespace Notifo.SDK.CommandQueue
{
    internal interface ICommand
    {
        ValueTask ExecuteAsync();

        bool Merge(ICommand other);
    }
}
