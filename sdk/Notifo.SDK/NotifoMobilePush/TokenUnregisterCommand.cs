// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
using Notifo.SDK.CommandQueue;

namespace Notifo.SDK.NotifoMobilePush
{
    internal sealed class TokenUnregisterCommand : ICommand
    {
        public string Token { get; set; }

        public async ValueTask ExecuteAsync(
            CancellationToken ct)
        {
            try
            {
                await NotifoIO.Current.MobilePush.DeleteMyTokenAsync(Token, ct);
            }
            catch (Exception ex)
            {
                NotifoIO.Current.RaiseError(ex.Message, ex, this);
                throw ex;
            }
        }

        public bool Merge(ICommand other)
        {
            return other is TokenUnregisterCommand;
        }
    }
}
